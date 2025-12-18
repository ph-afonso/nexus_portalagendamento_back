using Microsoft.Extensions.Logging;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;
using Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

namespace Nexus.ReportGen.Library.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de gerenciamento de RelatorioModeloPermissao
/// </summary>
public class RelatorioModeloPermissaoService : IRelatorioModeloPermissaoService
{
    private readonly IRelatorioModeloPermissaoRepository _relatoriomodelopermissaoRepository;
    private readonly ILogger<RelatorioModeloPermissaoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloPermissaoService(IRelatorioModeloPermissaoRepository relatoriomodelopermissaoRepository, ILogger<RelatorioModeloPermissaoService> logger)
    {
        _relatoriomodelopermissaoRepository = relatoriomodelopermissaoRepository ?? throw new ArgumentNullException(nameof(relatoriomodelopermissaoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioModeloPermissaoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriomodelopermissaoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloPermissaoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriomodelopermissaoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloPermissaoInputModel>> CreateAsync(RelatorioModeloPermissaoInputModel relatoriomodelopermissao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloPermissaoInputModel>();
        try
        {
            relatoriomodelopermissao.Operacao = "I"; // Insert

            retorno = await _relatoriomodelopermissaoRepository.SalvarAsync(relatoriomodelopermissao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloPermissao criado com sucesso. ID: {Id}", retorno.ResultData.CodRelatorioModelo);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriomodelopermissao");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloPermissaoInputModel>> UpdateAsync(RelatorioModeloPermissaoInputModel relatoriomodelopermissao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloPermissaoInputModel>();
        try
        {
            relatoriomodelopermissao.Operacao = "U"; // Update

            retorno = await _relatoriomodelopermissaoRepository.SalvarAsync(relatoriomodelopermissao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloPermissao atualizado com sucesso. ID: {Id}", retorno.ResultData.CodRelatorioModelo);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriomodelopermissao");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloPermissaoInputModel>> DeleteAsync(RelatorioModeloPermissaoInputModel relatoriomodelopermissao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloPermissaoInputModel>();
        try
        {
            relatoriomodelopermissao.Operacao = "D"; // Delete

            retorno = await _relatoriomodelopermissaoRepository.ExcluirAsync(relatoriomodelopermissao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloPermissao excluído com sucesso");
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao apagar o registro.");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }
}
