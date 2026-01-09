using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Helper;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces; 
namespace Nexus.PortalAgendamento.Library.Infrastructure.Services;

public class PortalAgendamentoService : IPortalAgendamentoService
{
    private readonly IPortalAgendamentoRepository _repository;
    private readonly ILogger<PortalAgendamentoService> _logger;
    private readonly PdfHelper _pdfHelper;

    public PortalAgendamentoService(
        IPortalAgendamentoRepository repository,
        ILogger<PortalAgendamentoService> logger,
        PdfHelper pdfHelper)
    {
        _repository = repository;
        _logger = logger;
        _pdfHelper = pdfHelper;
    }

    public async Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken ct)
        => (await _repository.GetNotasConhecimento(identificadorCliente, ct)).ResultData;

    public async Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken ct)
    {
        var result = await _repository.ValidarTokenAsync(model, ct);
        if (result.IsSuccess && result.ResultData != null)
        {
            var token = result.ResultData;
            token.TokenValido = (token.DataExpiracaoToken.HasValue && DateTime.Now <= token.DataExpiracaoToken.Value);
        }
        return result;
    }

    public async Task<NexusResult<AnaliseAnexoOutputModel>> UploadAnaliseAnexoAsync(Guid identificadorCliente, IFormFile arquivo, CancellationToken ct)
    {
        var result = new NexusResult<AnaliseAnexoOutputModel>();
        var output = new AnaliseAnexoOutputModel();

        var basePath = Environment.GetEnvironmentVariable("TRATATIVAS_BASE_PATH") ?? Path.GetTempPath();
        var tempPath = Path.Combine(basePath, "Nexus_Uploads");

        if (!Directory.Exists(tempPath))
            Directory.CreateDirectory(tempPath);

        var pdfFilePath = Path.Combine(tempPath, $"{identificadorCliente}.pdf");

        _logger.LogInformation($"[Upload] Salvando anexo em: {pdfFilePath}");

        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream, ct);
        }

        try
        {
            var (datas, horas) = _pdfHelper.ExtrairDadosDoPdf(pdfFilePath);

            output.DatasLocalizadas = datas.OrderBy(x => DateTime.Parse(x)).ToList();
            output.HorariosLocalizados = horas.OrderBy(x => x).ToList();

            if (!output.DatasLocalizadas.Any())
            {
                _logger.LogWarning("[Upload] Nenhuma data identificada no arquivo.");
                result.AddFailureMessage("Não foi possível identificar a data no arquivo.");
            }
            else
            {
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Upload] Erro fatal no processamento");
            result.AddFailureMessage("Falha técnica ao processar o arquivo.");
        }

        return result;
    }

    public async Task<NexusResult<ConfirmacaoOutputModel>> AgendarComAnexoTempAsync(ConfirmacaoInputModel input, DateTime dataSolicitada, CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        var basePath = Environment.GetEnvironmentVariable("TRATATIVAS_BASE_PATH") ?? Path.GetTempPath();
        var tempPath = Path.Combine(basePath, "Nexus_Uploads");

        var filePath = Path.Combine(tempPath, $"{input.IdentificadorCliente}.pdf");
        _logger.LogInformation($"[Agendar] Buscando anexo em: {filePath}");

        if (!File.Exists(filePath))
        {
            result.AddFailureMessage("Arquivo de anexo não encontrado ou expirado. Faça o upload novamente.");
            return result;
        }

        var arquivoBytes = await File.ReadAllBytesAsync(filePath, ct);

        var notasModel = await GetNotasConhecimento(input.IdentificadorCliente, ct);
        output.NotasFiscais = notasModel?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

        if (!output.NotasFiscais.Any())
        {
            result.AddFailureMessage("Notas não encontradas.");
            return result;
        }

        output.DataSugestao = dataSolicitada;

        var confirmacao = await ConfirmarAgendamento(input.IdentificadorCliente, dataSolicitada, output.NotasFiscais, ct);

        if (!confirmacao.IsSuccess)
        {
            foreach (var msg in confirmacao.Messages) result.AddFailureMessage(msg.Description);
            return result;
        }

        output.ResultadoProcessamento = confirmacao.ResultData ?? new List<AgendamentoDetalheModel>();

        var ocorrenciasProcessadas = new HashSet<int>();

        foreach (var item in output.ResultadoProcessamento.Where(x => x.Status == "Sucesso" || x.Status == "Já Agendado"))
        {
            try
            {
                var notaRef = output.NotasFiscais.FirstOrDefault(n => n.NrNotasFiscais == item.NrNota);
                if (notaRef?.CodFiliais == null) continue;

                var ocorrencia = await _repository.GetOcorrenciaAbertaAsync(item.NrNota, notaRef.CodFiliais.Value, ct);

                if (ocorrencia != null)
                {
                    if (ocorrenciasProcessadas.Contains(ocorrencia.IdOcorrencia))
                    {
                        _logger.LogInformation($"[Anexo] Ocorrência {ocorrencia.IdOcorrencia} já processada (Nota {item.NrNota}). Pulando upload duplicado.");
                        continue;
                    }

                    await _repository.VincularAnexoOcorrenciaAsync(
                        ocorrencia.IdOcorrencia,
                        "comprovante_agendamento.pdf",
                        arquivoBytes,
                        ct
                    );

                    ocorrenciasProcessadas.Add(ocorrencia.IdOcorrencia);
                    _logger.LogInformation($"[Anexo] Sucesso! Vinculado à ocorrência {ocorrencia.IdOcorrencia}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Anexo] Erro ao vincular na nota {item.NrNota}");
            }
        }

        // Montagem da Mensagem Final
        int qtdTotal = output.NotasFiscais.Count;
        int qtdNovos = output.ResultadoProcessamento.Count(x => x.Agendado);
        int qtdJaAgendado = output.ResultadoProcessamento.Count(x => string.Equals(x.Status, "Já Agendado", StringComparison.OrdinalIgnoreCase));
        int totalSucesso = qtdNovos + qtdJaAgendado;
        string dt = dataSolicitada.ToString("dd/MM/yyyy HH:mm");

        if (totalSucesso == qtdTotal && qtdTotal > 0)
        {
            output.Mensagem = qtdNovos > 0
                ? $"Sucesso! Agendamento confirmado para {dt}."
                : $"Agendamento já consta confirmado para {dt}.";
        }
        else if (totalSucesso > 0)
        {
            output.Mensagem = $"Agendamento parcial ({totalSucesso}/{qtdTotal}) realizado.";
        }
        else
        {
            output.Mensagem = "Não foi possível confirmar o agendamento.";
        }

        result.ResultData = output;
        result.AddDefaultSuccessMessage();

        // Opcional: Limpar arquivo
        try { File.Delete(filePath); } catch { }

        return result;
    }

    public async Task<NexusResult<List<AgendamentoDetalheModel>>> ConfirmarAgendamento(
        Guid identificadorCliente,
        DateTime dataAgendamento,
        List<NotaFiscalOutputModel> notas,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Iniciando confirmação CORE. Guid: {Guid}. Data: {Dt}. Qtd Notas: {Qtd}",
            identificadorCliente, dataAgendamento, notas.Count);

        var nexus = new NexusResult<List<AgendamentoDetalheModel>>();
        var listaResultados = new List<AgendamentoDetalheModel>();
        var notasAptasParaBanco = new List<NotaFiscalOutputModel>();

        string dataFmt = dataAgendamento.ToString("dd/MM/yyyy");
        string horaFmt = dataAgendamento.ToString("HH:mm");
        if (horaFmt == "00:00") horaFmt = "08:00";

        var cacheDecisaoCTe = new Dictionary<string, (string Status, string Mensagem, bool IsReagendamento, bool Gravar)>();

        foreach (var nota in notas)
        {
            if (nota.NrNotasFiscais == null || nota.CodFiliais == null) continue;

            var detalhe = CriarDetalheBase(nota);
            string chaveCTe = $"{nota.CodFiliais}-{nota.NrImpressoConhecimentos}";

            try
            {
                if (cacheDecisaoCTe.ContainsKey(chaveCTe))
                {
                    var decisao = cacheDecisaoCTe[chaveCTe];
                    detalhe.Status = decisao.Status;
                    detalhe.Mensagem = decisao.Mensagem;

                    if (decisao.Gravar)
                    {
                        nota.IsReagendamento = decisao.IsReagendamento;
                        if (detalhe.Status == "Já Agendado")
                        {
                            detalhe.DataAgendada = dataFmt;
                            detalhe.HoraAgendada = horaFmt;
                            detalhe.Agendado = false;
                        }
                        else
                        {
                            notasAptasParaBanco.Add(nota);
                        }
                    }
                    listaResultados.Add(detalhe);
                    continue;
                }

                var ocorrencia = await _repository.GetOcorrenciaAbertaAsync(nota.NrNotasFiscais.Value, nota.CodFiliais.Value, ct);
                bool isReagendamento = false;
                bool gravar = true;
                string statusDecisao = "Sucesso";
                string msgDecisao = "Agendamento Realizado";

                if (ocorrencia != null)
                {
                    var (podeEncerrar, mensagemBloqueio) = AnalisarOcorrencia(ocorrencia, dataAgendamento, horaFmt);

                    if (!string.IsNullOrEmpty(mensagemBloqueio))
                    {
                        statusDecisao = mensagemBloqueio == "JÁ AGENDADO" ? "Já Agendado" : "Bloqueado";
                        msgDecisao = mensagemBloqueio == "JÁ AGENDADO" ? "Agendamento confirmado." : $"Impedimento: {ocorrencia.CodOcorrenciaTipo}";

                        if (statusDecisao == "Já Agendado")
                        {
                            detalhe.DataAgendada = dataFmt;
                            detalhe.HoraAgendada = horaFmt;
                            gravar = true;
                        }
                        else
                        {
                            gravar = false;
                        }
                    }
                    else if (podeEncerrar)
                    {
                        await _repository.EncerrarOcorrenciaMassaAsync(ocorrencia, ct);
                        isReagendamento = (ocorrencia.CodOcorrenciaTipo == 91);
                        statusDecisao = "Sucesso";
                        msgDecisao = isReagendamento ? "Reagendamento Realizado" : "Agendamento Realizado";
                    }
                }

                cacheDecisaoCTe.Add(chaveCTe, (statusDecisao, msgDecisao, isReagendamento, gravar));

                detalhe.Status = statusDecisao;
                detalhe.Mensagem = msgDecisao;

                if (gravar && statusDecisao != "Bloqueado")
                {
                    if (statusDecisao == "Já Agendado")
                    {
                        detalhe.Agendado = false;
                    }
                    else
                    {
                        nota.IsReagendamento = isReagendamento;
                        notasAptasParaBanco.Add(nota);
                    }
                }
                listaResultados.Add(detalhe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar nota {Nota}", nota.NrNotasFiscais);
                detalhe.Status = "Erro Técnico";
                detalhe.Mensagem = "Falha na validação.";
                listaResultados.Add(detalhe);
            }
        }

        if (notasAptasParaBanco.Any())
        {
            var resultRepo = await _repository.RealizarAgendamentoAsync(new AgendamentoRepositoryInputModel
            {
                IdentificadorCliente = identificadorCliente,
                DataAgendamento = dataAgendamento,
                Notas = notasAptasParaBanco
            }, ct);

            foreach (var itemResult in listaResultados.Where(x => x.Status == "Sucesso"))
            {
                if (resultRepo.IsSuccess)
                {
                    itemResult.Agendado = true;
                    itemResult.DataAgendada = dataFmt;
                    itemResult.HoraAgendada = horaFmt;
                }
                else
                {
                    itemResult.Agendado = false;
                    itemResult.Status = "Erro Banco";
                    itemResult.Mensagem = "Falha ao gravar.";
                }
            }
        }

        if (listaResultados.Any())
        {
            await EnviarEmailResumoAsync(listaResultados, notas, dataAgendamento, ct);
        }

        nexus.AddData(listaResultados);
        nexus.AddDefaultSuccessMessage();
        return nexus;
    }

    private AgendamentoDetalheModel CriarDetalheBase(NotaFiscalOutputModel nota)
    {
        return new AgendamentoDetalheModel
        {
            NrNota = nota.NrNotasFiscais ?? 0,
            Serie = nota.NrSerieNotasFiscais ?? "",
            Pedido = nota.NrPedido ?? "-",
            NomeFornecedor = nota.NomeFornecedor ?? "Indefinido",
            NomeRecebedor = nota.NomeRecebedor ?? "Indefinido",
            Agendado = false
        };
    }

    private (bool encerrar, string bloqueio) AnalisarOcorrencia(OcorrenciaAbertaModel ocorrencia, DateTime dataSolicitada, string horaSolicitada)
    {
        if (ocorrencia.CodOcorrenciaTipo == 140 || ocorrencia.CodOcorrenciaTipo == 145)
            return (true, "");

        if (ocorrencia.CodOcorrenciaTipo == 91)
        {
            bool dataIgual = ocorrencia.DataAgendamento.HasValue &&
                             ocorrencia.DataAgendamento.Value.Date == dataSolicitada.Date;

            string horaBanco = ocorrencia.HoraAgendamento?.Trim() ?? "";
            bool horaIgual = false;

            if (!string.IsNullOrEmpty(horaBanco))
            {
                horaIgual = horaBanco.Equals(horaSolicitada, StringComparison.OrdinalIgnoreCase)
                            || horaBanco.StartsWith(horaSolicitada);
            }

            if (dataIgual && horaIgual)
            {
                return (false, "JÁ AGENDADO");
            }

            return (true, "");
        }

        return (false, "OCORRENCIA_IMPEDITIVA");
    }

    private async Task EnviarEmailResumoAsync(
        List<AgendamentoDetalheModel> resultados,
        List<NotaFiscalOutputModel> notasOriginais,
        DateTime dataSugestao,
        CancellationToken ct)
    {
        try
        {
            var notaRef = notasOriginais.FirstOrDefault();
            if (notaRef == null) return;

            var destinatarios = await _repository.GetDestinatariosEmailAsync(notaRef.CodClientesFornecedor, notaRef.CodClientesRecebedor, ct);
            var emailsTragetta = destinatarios
                .Select(d => d.Email?.Trim())
                .Where(e => !string.IsNullOrEmpty(e) && e.Contains("@tragetta", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(e => new { Email = e, Nome = "Equipe Tragetta", Login = "" })
                .ToList();

            if (!emailsTragetta.Any()) return;

            var nomeRecebedor = notaRef.NomeRecebedor ?? "Recebedor";
            var nomeFornecedor = notaRef.NomeFornecedor ?? "Fornecedor";

            var model = new EmailPostFixInputModel
            {
                DsAssunto = $"{nomeFornecedor} - PROCESSAMENTO DPA - {nomeRecebedor} - para {dataSugestao:dd/MM/yyyy HH:mm}",
                DsMensagem = EmailTemplateBuilder.BuildResumoProcessamento(resultados, nomeRecebedor),
                FlMensagemHtml = true,
                DestinatariosJson = JsonConvert.SerializeObject(emailsTragetta),
                EmailRemetente = "naoresponda@tragetta.srv.br",
                LoginRemetenteUsuarios = "PORTAL_AGENDAMENTO",
                CodEventos = null,
                CodRemetentePessoa = null,
                NmArquivo = null,
                Arquivo = null
            };
            await _repository.EnviarEmailPostfixAsync(model, ct);
        }
        catch (Exception ex) { _logger.LogError(ex, "Erro ao enviar e-mail de resumo"); }
    }
}