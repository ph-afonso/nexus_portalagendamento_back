using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
// Importando todo o stack do Nexus para garantir
using Nexus.Framework.Common;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Helper;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using Nexus.PortalAgendamento.Library.Infrastructure.Common;

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

    public async Task<ClienteOutputModel?> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetCliente(identificadorCliente, cancellationToken);
        return result.ResultData;
    }

    public async Task<NexusResult<bool>> CreateVoucherTratativa(Guid identificadorCliente, IFormFile file, CancellationToken cancellationToken = default)
    {
        return await _repository.CreateVoucherTratativaAsync(identificadorCliente, file, cancellationToken);
    }

    public async Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default)
    {
        return await _repository.UpdateDataAgendamento(model, cancellationToken);
    }

    //public async Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile file, CancellationToken cancellationToken = default)
    //{
    //    return await _repository.SendEmailAnexo(identificadorCliente, file, cancellationToken);
    //}

    public async Task<PortalAgendamentoOutputModel?> GetDataAgendamentoPdf(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return null;

        var tempPath = Path.GetTempFileName();
        try
        {
            using (var stream = File.Create(tempPath))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var imagens = _pdfHelper.ConverterPdfParaImagens(tempPath);
            var datasEncontradas = new List<DateTime>();

            foreach (var img in imagens)
            {
                var caminhoPre = _pdfHelper.PreprocessarImagemParaOCR(img);
                if (caminhoPre != null)
                {
                    var texto = _pdfHelper.ExtrairTextoDeImagem(caminhoPre);
                    var datas = _pdfHelper.ExtrairDatasDoTexto(texto);
                    datasEncontradas.AddRange(datas);

                    try { File.Delete(caminhoPre); } catch { }
                }

                try { File.Delete(img); } catch { }
            }

            return PdfHelper.CriarResultado(datasEncontradas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar PDF de agendamento.");
            return null;
        }
        finally
        {
            if (File.Exists(tempPath))
                try { File.Delete(tempPath); } catch { }
        }
    }

    //NOVOS
    public async Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken cancellationToken = default)
    {
        // 1. Chama o Repositório (que busca DtInclusao e DtAlteracao)
        var result = await _repository.ValidarTokenAsync(model, cancellationToken);

        // Se o repositório falhou ou não achou dados, repassa o erro/falha
        if (!result.IsSuccess || result.ResultData == null)
        {
            return result;
        }

        var output = result.ResultData;

        // 2. Aplica a Regra de Negócio
        // Como o 'output.DataVencimento' é uma propriedade calculada (Alteração ?? Inclusão + 48h),
        // só precisamos verificar se ela existe e comparar com Agora.

        if (output.DataExpiracaoToken.HasValue)
        {
            output.TokenValido = DateTime.Now <= output.DataExpiracaoToken.Value;
        }
        else
        {
            // Se não tem datas (banco inconsistente), é inválido por segurança
            output.TokenValido = false;
        }

        // O objeto 'result' mantém a referência ao 'output', então já está atualizado.
        return result;
    }

    public async Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetNotasConhecimento(identificadorCliente, cancellationToken);
        return result.ResultData;
    }

    public async Task<NexusResult<List<AgendamentoDetalheModel>>> ConfirmarAgendamento(Guid identificadorCliente, DateTime dataAgendamento, List<NotaFiscalOutputModel> notas, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<List<AgendamentoDetalheModel>>();
        var listaResultados = new List<AgendamentoDetalheModel>();
        var notasAptasParaBanco = new List<NotaFiscalOutputModel>();

        string dataFmt = dataAgendamento.ToString("dd/MM/yyyy");
        string horaFmt = "08:00";

        // -----------------------------------------------------------------------
        // PASSO 1: VALIDAÇÃO INDIVIDUAL
        // -----------------------------------------------------------------------
        foreach (var nota in notas)
        {
            if (nota.NrNotasFiscais == null || nota.CodFiliais == null) continue;

            // --- ATUALIZAÇÃO 1: Preenchendo dados completos para a tabela de email ---
            var detalhe = new AgendamentoDetalheModel
            {
                NrNota = nota.NrNotasFiscais.Value,
                Serie = nota.NrSerieNotasFiscais ?? "",
                Pedido = nota.NrPedido ?? "-",
                NomeFornecedor = nota.NomeFornecedor ?? "Indefinido",
                NomeRecebedor = nota.NomeRecebedor ?? "Indefinido",
                Agendado = false
            };

            nota.IsReagendamento = false;

            // Busca Ocorrência
            var ocorrencia = await _repository.GetOcorrenciaAbertaAsync(nota.NrNotasFiscais.Value, nota.CodFiliais.Value, cancellationToken);

            if (ocorrencia != null)
            {
                bool tentaEncerrar = false;

                if (ocorrencia.CodOcorrenciaTipo == 140 || ocorrencia.CodOcorrenciaTipo == 145)
                {
                    tentaEncerrar = true;
                }
                else if (ocorrencia.CodOcorrenciaTipo == 91)
                {
                    bool mesmaData = ocorrencia.DataAgendamento.HasValue && ocorrencia.DataAgendamento.Value.Date == dataAgendamento.Date;
                    bool mesmaHora = !string.IsNullOrEmpty(ocorrencia.HoraAgendamento) && ocorrencia.HoraAgendamento.Contains("08:00");

                    if (mesmaData && mesmaHora)
                    {
                        detalhe.Status = "Já Agendado";
                        detalhe.Mensagem = "Nota já possui agendamento confirmado.";
                        detalhe.DataAgendada = dataFmt;
                        detalhe.HoraAgendada = horaFmt;
                        listaResultados.Add(detalhe);
                        continue;
                    }

                    tentaEncerrar = true;
                    nota.IsReagendamento = true;
                }
                else
                {
                    detalhe.Status = "Bloqueado";
                    detalhe.Mensagem = $"Impedimento: Ocorrência {ocorrencia.CodOcorrenciaTipo} em aberto.";
                    listaResultados.Add(detalhe);
                    continue;
                }

                if (tentaEncerrar)
                {
                    try
                    {
                        await _repository.EncerrarOcorrenciaMassaAsync(ocorrencia, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        detalhe.Status = "Erro Técnico";
                        detalhe.Mensagem = $"Falha ao encerrar ocorrência {ocorrencia.CodOcorrenciaTipo}.";
                        _logger.LogError(ex, "Falha encerramento nota {Nota}", nota.NrNotasFiscais);
                        listaResultados.Add(detalhe);
                        continue;
                    }
                }
            }

            notasAptasParaBanco.Add(nota);
        }

        // -----------------------------------------------------------------------
        // PASSO 2: GRAVAÇÃO EM MASSA
        // -----------------------------------------------------------------------
        if (notasAptasParaBanco.Any())
        {
            var inputRepo = new AgendamentoRepositoryInputModel
            {
                IdentificadorCliente = identificadorCliente,
                DataAgendamento = dataAgendamento,
                Notas = notasAptasParaBanco
            };

            var resultRepo = await _repository.RealizarAgendamentoAsync(inputRepo, cancellationToken);

            foreach (var notaApta in notasAptasParaBanco)
            {
                // --- ATUALIZAÇÃO 2: Preenchendo dados completos também no sucesso ---
                var detalhe = new AgendamentoDetalheModel
                {
                    NrNota = notaApta.NrNotasFiscais!.Value,
                    Serie = notaApta.NrSerieNotasFiscais ?? "",
                    Pedido = notaApta.NrPedido ?? "-",
                    NomeFornecedor = notaApta.NomeFornecedor ?? "Indefinido",
                    NomeRecebedor = notaApta.NomeRecebedor ?? "Indefinido",

                    DataAgendada = dataFmt,
                    HoraAgendada = horaFmt
                };

                if (resultRepo.IsSuccess)
                {
                    detalhe.Agendado = true;
                    detalhe.Status = "Sucesso";
                    detalhe.Mensagem = notaApta.IsReagendamento ? "Reagendamento Realizado" : "Agendamento Realizado";
                }
                else
                {
                    detalhe.Agendado = false;
                    detalhe.Status = "Erro Banco";
                    detalhe.Mensagem = "Falha ao gravar agendamento no banco de dados.";
                }

                listaResultados.Add(detalhe);
            }
        }

        // -----------------------------------------------------------------------
        // PASSO 3: ENVIAR EMAIL
        // -----------------------------------------------------------------------
        if (listaResultados.Any())
        {
            await EnviarEmailResumoAsync(listaResultados, notas, dataAgendamento, cancellationToken);
        }

        nexus.AddData(listaResultados);
        nexus.AddDefaultSuccessMessage();
        return nexus;
    }

    // ------------------------------------------------------------------------------------------------
    // MÉTODO ATUALIZADO: EnviarEmailResumoAsync (Serializa objetos e passa nome Solicitante)
    // ------------------------------------------------------------------------------------------------
    private async Task EnviarEmailResumoAsync(
    List<AgendamentoDetalheModel> resultados,
    List<NotaFiscalOutputModel> notasOriginais,
    DateTime dataSugestao, // <--- NOVO PARÂMETRO
    CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obter dados
            var notaRef = notasOriginais.FirstOrDefault();
            if (notaRef == null) return;

            var codFornecedor = notaRef.CodClientesFornecedor;
            var codRecebedor = notaRef.CodClientesRecebedor;

            var nomeFornecedor = notaRef.NomeFornecedor ?? "Fornecedor";
            var nomeRecebedor = notaRef.NomeRecebedor ?? "Recebedor";

            // 2. Buscar Destinatários
            var destinatarios = await _repository.GetDestinatariosEmailAsync(codFornecedor, codRecebedor, cancellationToken);

            // 3. Filtra emails
            var emails = destinatarios
                .Select(d => d.Email?.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e) && e.Contains("@tragetta", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emails.Count == 0)
            {
                _logger.LogWarning("Resumo Agendamento: Nenhum destinatário @tragetta encontrado.");
                return;
            }

            var listaParaJson = emails.Select(e => new
            {
                Email = e,
                Nome = "Equipe Tragetta",
                Login = ""
            }).ToList();

            // 4. Montar Assunto (USANDO A DATA DE SUGESTÃO)
            // Antes era: DateTime.Now
            var assunto = $"{nomeFornecedor} - PROCESSAMENTO DPA - {nomeRecebedor} - para {dataSugestao:dd/MM/yyyy}";

            var htmlBody = EmailTemplateBuilder.BuildResumoProcessamento(resultados, nomeRecebedor);

            // 5. Montar Input Model
            var model = new EmailPostFixInputModel
            {
                DsAssunto = assunto,
                DsMensagem = htmlBody,
                FlMensagemHtml = true,
                DestinatariosJson = JsonConvert.SerializeObject(listaParaJson),
                EmailRemetente = "naoresponda@tragetta.srv.br",
                LoginRemetenteUsuarios = "PORTAL_AGENDAMENTO",
                CodEventos = null,
                CodRemetentePessoa = null,
                NmArquivo = null,
                MimeType = null,
                Arquivo = null
            };

            // 6. Enviar
            await _repository.EnviarEmailPostfixAsync(model, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EnviarEmailResumoAsync -> Falha ao enviar email.");
        }
    }
}