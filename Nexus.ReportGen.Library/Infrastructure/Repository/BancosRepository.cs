using Microsoft.Extensions.Logging;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Framework.Data.Repository.Default;
using Nexus.Framework.Data.Repository.Interfaces;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Constants;

namespace Nexus.Sample.Library.Infrastructure.Repository;

/// <summary>
/// Implementação do repositório de Bancos usando ProcedureRepository
/// </summary>
public class BancosRepository : ProcedureRepository, IBancosRepository
{
    private readonly IServiceBase _serviceBase;

    /// <summary>
    /// Construtor
    /// </summary>
    public BancosRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
    }

    /// <inheritdoc />
    public async Task<NexusResult<BancosOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetByBancos -> Buscando bancos para ID: {id}", id);

            var result = await _serviceBase.FindByNumericIdAsync<BancosOutputModel>(
                BancosProcedures.SNG_TB_BANCOS,
                "COD_BANCOS",
                id);

            _logger.LogInformation("GetByBancos -> Resultado obtido com sucesso para ID: {id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByBancos -> Erro ao buscar bancos para ID: {id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<BancosOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetList -> Buscando lista paginada com filtros: {@filter}", filter);

            var resultado = await _serviceBase.ExecutePaginatedQueryAsync<BancosOutputModel>(
                BancosProcedures.LST_TB_BANCOS,
                filter);

            _logger.LogInformation("GetList -> Lista obtida com sucesso. Total de registros: {count}",
                resultado?.ResultData?.Items?.Count() ?? 0);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetList -> Erro ao buscar lista paginada: {@filter}", filter);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<BancosInputModel>> SalvarAsync(BancosInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Salvar -> Processando salvamento de bancos: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Salvar -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Salvar -> Bancos salvo com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salvar -> Erro ao salvar bancos: {@model}", model);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<BancosInputModel>> ExcluirAsync(BancosInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Excluir -> Processando exclusão de bancos: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Excluir -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Excluir -> Bancos excluído com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excluir -> Erro ao excluir bancos: {@model}", model);
            throw;
        }
    }

}
