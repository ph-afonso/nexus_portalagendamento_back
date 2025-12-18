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
/// Implementação do serviço de gerenciamento de TipoParametro
/// </summary>
public class TipoParametroService : ITipoParametroService
{
    private readonly ITipoParametroRepository _tipoparametroRepository;
    private readonly ILogger<TipoParametroService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public TipoParametroService(ITipoParametroRepository tipoparametroRepository, ILogger<TipoParametroService> logger)
    {
        _tipoparametroRepository = tipoparametroRepository ?? throw new ArgumentNullException(nameof(tipoparametroRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TipoParametroOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _tipoparametroRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<TipoParametroOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tipoparametroRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<TipoParametroInputModel>> CreateAsync(TipoParametroInputModel tipoparametro, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<TipoParametroInputModel>();
        try
        {
            tipoparametro.Operacao = "I"; // Insert

            retorno = await _tipoparametroRepository.SalvarAsync(tipoparametro, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("TipoParametro criado com sucesso. ID: {Id}", retorno.ResultData.CodTipoParametro);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar tipoparametro");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<TipoParametroInputModel>> UpdateAsync(TipoParametroInputModel tipoparametro, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<TipoParametroInputModel>();
        try
        {
            tipoparametro.Operacao = "U"; // Update

            retorno = await _tipoparametroRepository.SalvarAsync(tipoparametro, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("TipoParametro atualizado com sucesso. ID: {Id}", retorno.ResultData.CodTipoParametro);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar tipoparametro");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<TipoParametroInputModel>> DeleteAsync(TipoParametroInputModel tipoparametro, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<TipoParametroInputModel>();
        try
        {
            tipoparametro.Operacao = "D"; // Delete

            retorno = await _tipoparametroRepository.ExcluirAsync(tipoparametro, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("TipoParametro excluído com sucesso");
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
