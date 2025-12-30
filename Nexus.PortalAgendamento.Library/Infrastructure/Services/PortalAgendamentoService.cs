using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services;

public class PortalAgendamentoService : IPortalAgendamentoService
{
    private readonly IPortalAgendamentoRepository _repository;
    private readonly ILogger<PortalAgendamentoService> _logger;

    public PortalAgendamentoService(
        IPortalAgendamentoRepository repository,
        ILogger<PortalAgendamentoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken ct)
        => (await _repository.GetNotasConhecimento(identificadorCliente, ct)).ResultData;

    // --- CORREÇÃO AQUI: Lógica de Validação do Token ---
    public async Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken ct)
    {
        // 1. Busca dados no repositório
        var result = await _repository.ValidarTokenAsync(model, ct);

        // 2. Aplica regra de negócio (Data Atual vs Validade)
        if (result.IsSuccess && result.ResultData != null)
        {
            var token = result.ResultData;

            // Se tem data de expiração e ela é maior que agora, o token é válido
            if (token.DataExpiracaoToken.HasValue && DateTime.Now <= token.DataExpiracaoToken.Value)
            {
                token.TokenValido = true;
            }
            else
            {
                token.TokenValido = false; // Expirado ou data inválida
            }
        }

        return result;
    }

    public async Task<NexusResult<List<AgendamentoDetalheModel>>> ConfirmarAgendamento(
        Guid identificadorCliente,
        DateTime dataAgendamento,
        List<NotaFiscalOutputModel> notas,
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Service] Iniciando confirmação para ClienteGUID: {Guid}. Qtd Notas: {Qtd}", identificadorCliente, notas.Count);

        var nexus = new NexusResult<List<AgendamentoDetalheModel>>();
        var listaResultados = new List<AgendamentoDetalheModel>();
        var notasAptasParaBanco = new List<NotaFiscalOutputModel>();

        string dataFmt = dataAgendamento.ToString("dd/MM/yyyy");
        string horaFmt = "08:00";

        // -----------------------------------------------------------------------
        // FASE 1: VALIDAÇÃO (Regras de Ocorrências)
        // -----------------------------------------------------------------------
        foreach (var nota in notas)
        {
            if (nota.NrNotasFiscais == null || nota.CodFiliais == null) continue;

            var detalhe = CriarDetalheBase(nota);

            try
            {
                var ocorrencia = await _repository.GetOcorrenciaAbertaAsync(nota.NrNotasFiscais.Value, nota.CodFiliais.Value, ct);

                if (ocorrencia != null)
                {
                    var (podeEncerrar, mensagemBloqueio) = AnalisarOcorrencia(ocorrencia, dataAgendamento);

                    if (!string.IsNullOrEmpty(mensagemBloqueio))
                    {
                        detalhe.Status = mensagemBloqueio == "JÁ AGENDADO" ? "Já Agendado" : "Bloqueado";
                        detalhe.Mensagem = mensagemBloqueio == "JÁ AGENDADO" ? "Nota já possui agendamento confirmado." : $"Impedimento: {ocorrencia.CodOcorrenciaTipo}";

                        if (detalhe.Status == "Já Agendado")
                        {
                            detalhe.DataAgendada = dataFmt;
                            detalhe.HoraAgendada = horaFmt;
                        }

                        listaResultados.Add(detalhe);
                        continue;
                    }

                    if (podeEncerrar)
                    {
                        await _repository.EncerrarOcorrenciaMassaAsync(ocorrencia, ct);
                        nota.IsReagendamento = (ocorrencia.CodOcorrenciaTipo == 91);
                    }
                }

                notasAptasParaBanco.Add(nota);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] Erro ao validar nota {Nota}", nota.NrNotasFiscais);
                detalhe.Status = "Erro Técnico";
                detalhe.Mensagem = "Falha na validação.";
                listaResultados.Add(detalhe);
            }
        }

        // -----------------------------------------------------------------------
        // FASE 2: PERSISTÊNCIA
        // -----------------------------------------------------------------------
        if (notasAptasParaBanco.Any())
        {
            var resultRepo = await _repository.RealizarAgendamentoAsync(new AgendamentoRepositoryInputModel
            {
                IdentificadorCliente = identificadorCliente,
                DataAgendamento = dataAgendamento,
                Notas = notasAptasParaBanco
            }, ct);

            foreach (var notaApta in notasAptasParaBanco)
            {
                var detalhe = CriarDetalheBase(notaApta);
                detalhe.DataAgendada = dataFmt;
                detalhe.HoraAgendada = horaFmt;

                if (resultRepo.IsSuccess)
                {
                    detalhe.Agendado = true;
                    detalhe.Status = "Sucesso";
                    detalhe.Mensagem = notaApta.IsReagendamento ? "Reagendamento Realizado" : "Agendamento Realizado";
                }
                else
                {
                    detalhe.Status = "Erro Banco";
                    detalhe.Mensagem = "Falha ao gravar.";
                }
                listaResultados.Add(detalhe);
            }
        }

        // -----------------------------------------------------------------------
        // FASE 3: NOTIFICAÇÃO
        // -----------------------------------------------------------------------
        if (listaResultados.Any())
        {
            await EnviarEmailResumoAsync(listaResultados, notas, dataAgendamento, ct);
        }

        nexus.AddData(listaResultados);
        nexus.AddDefaultSuccessMessage();
        return nexus;
    }

    // --- MÉTODOS AUXILIARES ---

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

    private (bool encerrar, string bloqueio) AnalisarOcorrencia(OcorrenciaAbertaModel ocorrencia, DateTime dataSolicitada)
    {
        if (ocorrencia.CodOcorrenciaTipo == 140 || ocorrencia.CodOcorrenciaTipo == 145)
            return (true, "");

        if (ocorrencia.CodOcorrenciaTipo == 91)
        {
            bool mesmaData = ocorrencia.DataAgendamento.HasValue && ocorrencia.DataAgendamento.Value.Date == dataSolicitada.Date;
            bool mesmaHora = !string.IsNullOrEmpty(ocorrencia.HoraAgendamento) && ocorrencia.HoraAgendamento.Contains("08:00");

            if (mesmaData && mesmaHora) return (false, "JÁ AGENDADO");

            return (true, ""); // É Reagendamento
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

            if (!emailsTragetta.Any())
            {
                _logger.LogWarning("[Service] Email ignorado: Nenhum destinatário @tragetta válido encontrado.");
                return;
            }

            var nomeRecebedor = notaRef.NomeRecebedor ?? "Recebedor";
            var nomeFornecedor = notaRef.NomeFornecedor ?? "Fornecedor";

            var model = new EmailPostFixInputModel
            {
                DsAssunto = $"{nomeFornecedor} - PROCESSAMENTO DPA - {nomeRecebedor} - para {dataSugestao:dd/MM/yyyy}",
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Service] Falha não impeditiva no envio de email.");
        }
    }
}