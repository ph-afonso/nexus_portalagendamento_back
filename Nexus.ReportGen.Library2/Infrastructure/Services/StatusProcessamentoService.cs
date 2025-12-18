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
/// Implementação do serviço de gerenciamento de StatusProcessamento
/// </summary>
public class StatusProcessamentoService : IStatusProcessamentoService
{
    private readonly IStatusProcessamentoRepository _statusprocessamentoRepository;
    private readonly ILogger<StatusProcessamentoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public StatusProcessamentoService(IStatusProcessamentoRepository statusprocessamentoRepository, ILogger<StatusProcessamentoService> logger)
    {
        _statusprocessamentoRepository = statusprocessamentoRepository ?? throw new ArgumentNullException(nameof(statusprocessamentoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<StatusProcessamentoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _statusprocessamentoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<StatusProcessamentoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _statusprocessamentoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<StatusProcessamentoInputModel>> CreateAsync(StatusProcessamentoInputModel statusprocessamento, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<StatusProcessamentoInputModel>();
        try
        {
            statusprocessamento.Operacao = "I"; // Insert

            retorno = await _statusprocessamentoRepository.SalvarAsync(statusprocessamento, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("StatusProcessamento criado com sucesso. ID: {Id}", retorno.ResultData.CodStatusProcessamento);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar statusprocessamento");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<StatusProcessamentoInputModel>> UpdateAsync(StatusProcessamentoInputModel statusprocessamento, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<StatusProcessamentoInputModel>();
        try
        {
            statusprocessamento.Operacao = "U"; // Update

            retorno = await _statusprocessamentoRepository.SalvarAsync(statusprocessamento, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("StatusProcessamento atualizado com sucesso. ID: {Id}", retorno.ResultData.CodStatusProcessamento);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar statusprocessamento");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<StatusProcessamentoInputModel>> DeleteAsync(StatusProcessamentoInputModel statusprocessamento, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<StatusProcessamentoInputModel>();
        try
        {
            statusprocessamento.Operacao = "D"; // Delete

            retorno = await _statusprocessamentoRepository.ExcluirAsync(statusprocessamento, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("StatusProcessamento excluído com sucesso");
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
