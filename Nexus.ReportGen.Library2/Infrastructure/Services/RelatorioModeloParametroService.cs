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
/// Implementação do serviço de gerenciamento de RelatorioModeloParametro
/// </summary>
public class RelatorioModeloParametroService : IRelatorioModeloParametroService
{
    private readonly IRelatorioModeloParametroRepository _relatoriomodeloparametroRepository;
    private readonly ILogger<RelatorioModeloParametroService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloParametroService(IRelatorioModeloParametroRepository relatoriomodeloparametroRepository, ILogger<RelatorioModeloParametroService> logger)
    {
        _relatoriomodeloparametroRepository = relatoriomodeloparametroRepository ?? throw new ArgumentNullException(nameof(relatoriomodeloparametroRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioModeloParametroOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriomodeloparametroRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloParametroOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriomodeloparametroRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloParametroInputModel>> CreateAsync(RelatorioModeloParametroInputModel relatoriomodeloparametro, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloParametroInputModel>();
        try
        {
            relatoriomodeloparametro.Operacao = "I"; // Insert

            retorno = await _relatoriomodeloparametroRepository.SalvarAsync(relatoriomodeloparametro, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloParametro criado com sucesso. ID: {Id}", retorno.ResultData.CodParametro);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriomodeloparametro");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloParametroInputModel>> UpdateAsync(RelatorioModeloParametroInputModel relatoriomodeloparametro, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloParametroInputModel>();
        try
        {
            relatoriomodeloparametro.Operacao = "U"; // Update

            retorno = await _relatoriomodeloparametroRepository.SalvarAsync(relatoriomodeloparametro, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloParametro atualizado com sucesso. ID: {Id}", retorno.ResultData.CodParametro);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriomodeloparametro");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloParametroInputModel>> DeleteAsync(RelatorioModeloParametroInputModel relatoriomodeloparametro, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloParametroInputModel>();
        try
        {
            relatoriomodeloparametro.Operacao = "D"; // Delete

            retorno = await _relatoriomodeloparametroRepository.ExcluirAsync(relatoriomodeloparametro, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloParametro excluído com sucesso");
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
