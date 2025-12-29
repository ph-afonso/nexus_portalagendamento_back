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

    public async Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetNotasConhecimento(identificadorCliente, cancellationToken);
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

    public async Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile file, CancellationToken cancellationToken = default)
    {
        return await _repository.SendEmailAnexo(identificadorCliente, file, cancellationToken);
    }

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
}