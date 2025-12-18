using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Domain;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Repository;
using Nexus.Sample.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using System.Drawing;
using System;
using System.Text.RegularExpressions;
using Tesseract;
using System.Text;
using System.Globalization;
using System.Drawing.Imaging;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;




namespace Nexus.Sample.Library.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de gerenciamento de Bancos
/// </summary>
public class PortalAgendamentoService : IPortalAgendamentoService
{
    private readonly IPortalAgendamentoRepository _portalAgendamentoRepository;
    private readonly ILogger<PortalAgendamentoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public PortalAgendamentoService(IPortalAgendamentoRepository portalAgendamentoRepository, ILogger<PortalAgendamentoService> logger)
    {
        _portalAgendamentoRepository = portalAgendamentoRepository ?? throw new ArgumentNullException(nameof(portalAgendamentoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    /// <inheritdoc />
    public async Task<ClienteOutputModel?> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _portalAgendamentoRepository.GetCliente(identificadorCliente, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter data validade token");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PortalAgendamentoOutputModel?> GetDataAgendamentoConfirmacao(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _portalAgendamentoRepository.GetDataAgendamentoConfirmacao(identificadorCliente, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter data validade token");
            throw;
        }
    }

    public async Task<PortalAgendamentoOutputModel?> GetDataAgendamentoPdf(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return null;

        string textoCompletoDoPdf = "";
        var datasEncontradas = new List<DateTime>();

        try
        {
            await using var stream = file.OpenReadStream();

            try
            {
                var pdfReader = new iText.Kernel.Pdf.PdfReader(stream);
                var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader);

                var strategies = new List<ITextExtractionStrategy>
            {
                new SimpleTextExtractionStrategy(),
                new LocationTextExtractionStrategy()
            };

                foreach (var strategy in strategies)
                {
                    string textoCompleto = "";
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var textoPage = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), strategy);
                        textoCompleto += textoPage + Environment.NewLine;
                    }

                    if (!string.IsNullOrEmpty(textoCompleto) && textoCompleto.Length > 50)
                    {
                        textoCompletoDoPdf = textoCompleto;
                        _logger.LogInformation($"Extração direta bem-sucedida com {strategy.GetType().Name}");
                        break;
                    }
                }

                pdfDocument.Close();

                if (!string.IsNullOrEmpty(textoCompletoDoPdf))
                {
                    datasEncontradas = PdfHelper.ExtrairDatasDoTexto(textoCompletoDoPdf);
                    if (datasEncontradas.Any())
                    {
                        _logger.LogInformation($"Encontradas {datasEncontradas.Count} datas via extração direta");
                        return PdfHelper.CriarResultado(datasEncontradas);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha na extração direta de texto. Prosseguindo com OCR.");
            }

            _logger.LogInformation("Iniciando extração via OCR...");

            stream.Position = 0;
            var caminhoPdf = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}.pdf");

            await using (var fileStream = new FileStream(caminhoPdf, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            try
            {
                var dpisParaTestar = new int[] { 300, 200, 150 };

                foreach (var dpi in dpisParaTestar)
                {
                    _logger.LogInformation($"Tentando conversão com DPI {dpi}");
                    var caminhosImagens = PdfHelper.ConverterPdfParaImagens(caminhoPdf, dpi);

                    if (caminhosImagens.Any())
                    {
                        foreach (var caminhoImagem in caminhosImagens)
                        {
                            var textoOriginal = PdfHelper.ExtrairTextoDeImagem(caminhoImagem);
                            if (!string.IsNullOrEmpty(textoOriginal))
                            {
                                textoCompletoDoPdf += textoOriginal + Environment.NewLine;
                            }

                            var imagemPreprocessada = PdfHelper.PreprocessarImagemParaOCR(caminhoImagem);
                            if (!string.IsNullOrEmpty(imagemPreprocessada))
                            {
                                var textoPreprocessado = PdfHelper.ExtrairTextoDeImagem(imagemPreprocessada);
                                if (!string.IsNullOrEmpty(textoPreprocessado))
                                {
                                    textoCompletoDoPdf += textoPreprocessado + Environment.NewLine;
                                }

                                if (File.Exists(imagemPreprocessada))
                                    File.Delete(imagemPreprocessada);
                            }
                        }

                        foreach (var caminho in caminhosImagens)
                        {
                            if (File.Exists(caminho)) File.Delete(caminho);
                        }

                        if (!string.IsNullOrEmpty(textoCompletoDoPdf))
                        {
                            datasEncontradas = PdfHelper.ExtrairDatasDoTexto(textoCompletoDoPdf);
                            if (datasEncontradas.Any())
                            {
                                _logger.LogInformation($"Encontradas {datasEncontradas.Count} datas via OCR com DPI {dpi}");
                                break;
                            }
                        }
                    }

                    if (datasEncontradas.Any()) break;
                }
            }
            finally
            {
                if (File.Exists(caminhoPdf))
                    File.Delete(caminhoPdf);
            }

            return datasEncontradas.Any() ? PdfHelper.CriarResultado(datasEncontradas) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro geral ao processar PDF.");
            return null;
        }
    }


    /// <inheritdoc />
    public async Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _portalAgendamentoRepository.GetNotasConhecimento(identificadorCliente, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro a obter notas do conhecimento");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PortalAgendamentoOutputModel?> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _portalAgendamentoRepository.GetValidadeToken(identificadorCliente, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter data validade token");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile request, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<EmailPostFixInputModel>();
        try
        {

            retorno = await _portalAgendamentoRepository.SendEmailAnexo(identificadorCliente, request, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("Email enviado com sucesso.");
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<PortalAgendamentoInputModel>();
        try
        {
            model.Operacao = "U";

            retorno = await _portalAgendamentoRepository.UpdateDataAgendamento(model, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("Data agendamento atualizada com sucesso.");
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar data");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <summary>
    /// Cria a tratativa de ocorrência "VOUCHER", persiste arquivo no servidor e, em caso de falha,
    /// delega ao repositório o envio de e-mail de fallback (Postfix) com o VOUCHER em anexo.
    /// </summary>
    /// <param name="identificadorCliente">Identificador do cliente (token do portal)</param>
    /// <param name="request">Arquivo PDF do VOUCHER</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>NexusResult indicando sucesso ou falha</returns>
    public async Task<NexusResult<bool>> CreateVoucherTratativaAsync(Guid identificadorCliente, IFormFile request, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<bool>();

        try
        {
            if (identificadorCliente == Guid.Empty || request is null || request.Length == 0)
            {
                retorno.AddFailureMessage("IdentificadorCliente e arquivo são obrigatórios.");
                return retorno;
            }

            var operacao = await _portalAgendamentoRepository.CreateVoucherTratativaAsync(identificadorCliente, request, cancellationToken);

            if (operacao.IsSuccess)
            {
                _logger.LogInformation("Tratativa VOUCHER criada e arquivo anexado com sucesso. IdentificadorCliente={identificadorCliente}", identificadorCliente);
                operacao.AddDefaultSuccessMessage();
                return operacao;
            }

            operacao.AddDefaultFailureMessage();
            return operacao;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar tratativa VOUCHER");
            retorno.AddFailureMessage(ex.Message);
            return retorno;
        }
    }



}
