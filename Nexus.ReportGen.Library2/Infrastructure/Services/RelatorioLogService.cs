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
/// Implementação do serviço de gerenciamento de RelatorioLog
/// </summary>
public class RelatorioLogService : IRelatorioLogService
{
    private readonly IRelatorioLogRepository _relatoriologRepository;
    private readonly ILogger<RelatorioLogService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioLogService(IRelatorioLogRepository relatoriologRepository, ILogger<RelatorioLogService> logger)
    {
        _relatoriologRepository = relatoriologRepository ?? throw new ArgumentNullException(nameof(relatoriologRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioLogOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriologRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioLogOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriologRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioLogInputModel>> CreateAsync(RelatorioLogInputModel relatoriolog, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioLogInputModel>();
        try
        {
            relatoriolog.Operacao = "I"; // Insert

            retorno = await _relatoriologRepository.SalvarAsync(relatoriolog, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioLog criado com sucesso. ID: {Id}", retorno.ResultData.CodLog);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriolog");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioLogInputModel>> UpdateAsync(RelatorioLogInputModel relatoriolog, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioLogInputModel>();
        try
        {
            relatoriolog.Operacao = "U"; // Update

            retorno = await _relatoriologRepository.SalvarAsync(relatoriolog, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioLog atualizado com sucesso. ID: {Id}", retorno.ResultData.CodLog);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriolog");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioLogInputModel>> DeleteAsync(RelatorioLogInputModel relatoriolog, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioLogInputModel>();
        try
        {
            relatoriolog.Operacao = "D"; // Delete

            retorno = await _relatoriologRepository.ExcluirAsync(relatoriolog, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioLog excluído com sucesso");
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
