using Microsoft.Extensions.Logging;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Sample.Library.Infrastructure.Domain;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;

namespace Nexus.Sample.Library.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de gerenciamento de Bancos
/// </summary>
public class BancosService : IBancosService
{
    private readonly IBancosRepository _bancosRepository;
    private readonly ILogger<BancosService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public BancosService(IBancosRepository bancosRepository, ILogger<BancosService> logger)
    {
        _bancosRepository = bancosRepository ?? throw new ArgumentNullException(nameof(bancosRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<BancosOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _bancosRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<BancosOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _bancosRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<BancosInputModel>> CreateAsync(BancosInputModel bancos, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<BancosInputModel>();
        try
        {
            bancos.Operacao = "I"; // Insert
            bancos.FlExcluido = false;

            retorno = await _bancosRepository.SalvarAsync(bancos, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("Bancos criado com sucesso. ID: {Id}", retorno.ResultData.CodBancos);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar bancos");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<BancosInputModel>> UpdateAsync(BancosInputModel bancos, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<BancosInputModel>();
        try
        {
            bancos.Operacao = "U"; // Update

            retorno = await _bancosRepository.SalvarAsync(bancos, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("Bancos atualizado com sucesso. ID: {Id}", retorno.ResultData.CodBancos);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar bancos");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<BancosInputModel>> DeleteAsync(BancosInputModel bancos, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<BancosInputModel>();
        try
        {
            bancos.Operacao = "D"; // Delete

            retorno = await _bancosRepository.ExcluirAsync(bancos, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("Bancos excluído com sucesso");
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
