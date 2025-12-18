using Microsoft.Extensions.Logging;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Framework.Data.Repository.Default;
using Nexus.Framework.Data.Repository.Interfaces;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;
using Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;
using Nexus.ReportGen.Library.Infrastructure.Constants;

namespace Nexus.ReportGen.Library.Infrastructure.Repository;

/// <summary>
/// Implementação do repositório de RelatorioModeloDestino usando ProcedureRepository
/// </summary>
public class RelatorioModeloDestinoRepository : ProcedureRepository, IRelatorioModeloDestinoRepository
{
    private readonly IServiceBase _serviceBase;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloDestinoRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloDestinoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetByRelatorioModeloDestino -> Buscando relatoriomodelodestino para ID: {id}", id);

            var result = await _serviceBase.FindByNumericIdAsync<RelatorioModeloDestinoOutputModel>(
                RelatorioModeloDestinoProcedures.SNG_TB_RELATORIO_MODELO_DESTINO,
                "COD_MODELO_DESTINO",
                id);

            _logger.LogInformation("GetByRelatorioModeloDestino -> Resultado obtido com sucesso para ID: {id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByRelatorioModeloDestino -> Erro ao buscar relatoriomodelodestino para ID: {id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloDestinoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetList -> Buscando lista paginada com filtros: {@filter}", filter);

            var resultado = await _serviceBase.ExecutePaginatedQueryAsync<RelatorioModeloDestinoOutputModel>(
                RelatorioModeloDestinoProcedures.LST_TB_RELATORIO_MODELO_DESTINO,
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
    public async Task<NexusResult<RelatorioModeloDestinoInputModel>> SalvarAsync(RelatorioModeloDestinoInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Salvar -> Processando salvamento de relatoriomodelodestino: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Salvar -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Salvar -> RelatorioModeloDestino salvo com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salvar -> Erro ao salvar relatoriomodelodestino: {@model}", model);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloDestinoInputModel>> ExcluirAsync(RelatorioModeloDestinoInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Excluir -> Processando exclusão de relatoriomodelodestino: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Excluir -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Excluir -> RelatorioModeloDestino excluído com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excluir -> Erro ao excluir relatoriomodelodestino: {@model}", model);
            throw;
        }
    }

}
