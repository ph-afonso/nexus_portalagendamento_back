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
/// Implementação do serviço de gerenciamento de RelatorioModeloSaida
/// </summary>
public class RelatorioModeloSaidaService : IRelatorioModeloSaidaService
{
    private readonly IRelatorioModeloSaidaRepository _relatoriomodelosaidaRepository;
    private readonly ILogger<RelatorioModeloSaidaService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloSaidaService(IRelatorioModeloSaidaRepository relatoriomodelosaidaRepository, ILogger<RelatorioModeloSaidaService> logger)
    {
        _relatoriomodelosaidaRepository = relatoriomodelosaidaRepository ?? throw new ArgumentNullException(nameof(relatoriomodelosaidaRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioModeloSaidaOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriomodelosaidaRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloSaidaOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriomodelosaidaRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloSaidaInputModel>> CreateAsync(RelatorioModeloSaidaInputModel relatoriomodelosaida, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloSaidaInputModel>();
        try
        {
            relatoriomodelosaida.Operacao = "I"; // Insert

            retorno = await _relatoriomodelosaidaRepository.SalvarAsync(relatoriomodelosaida, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloSaida criado com sucesso. ID: {Id}", retorno.ResultData.CodModeloSaida);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriomodelosaida");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloSaidaInputModel>> UpdateAsync(RelatorioModeloSaidaInputModel relatoriomodelosaida, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloSaidaInputModel>();
        try
        {
            relatoriomodelosaida.Operacao = "U"; // Update

            retorno = await _relatoriomodelosaidaRepository.SalvarAsync(relatoriomodelosaida, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloSaida atualizado com sucesso. ID: {Id}", retorno.ResultData.CodModeloSaida);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriomodelosaida");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloSaidaInputModel>> DeleteAsync(RelatorioModeloSaidaInputModel relatoriomodelosaida, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloSaidaInputModel>();
        try
        {
            relatoriomodelosaida.Operacao = "D"; // Delete

            retorno = await _relatoriomodelosaidaRepository.ExcluirAsync(relatoriomodelosaida, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloSaida excluído com sucesso");
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
