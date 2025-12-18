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
/// Implementação do repositório de RelatorioModeloParametro usando ProcedureRepository
/// </summary>
public class RelatorioModeloParametroRepository : ProcedureRepository, IRelatorioModeloParametroRepository
{
    private readonly IServiceBase _serviceBase;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloParametroRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloParametroOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetByRelatorioModeloParametro -> Buscando relatoriomodeloparametro para ID: {id}", id);

            var result = await _serviceBase.FindByNumericIdAsync<RelatorioModeloParametroOutputModel>(
                RelatorioModeloParametroProcedures.SNG_TB_RELATORIO_MODELO_PARAMETRO,
                "COD_PARAMETRO",
                id);

            _logger.LogInformation("GetByRelatorioModeloParametro -> Resultado obtido com sucesso para ID: {id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByRelatorioModeloParametro -> Erro ao buscar relatoriomodeloparametro para ID: {id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloParametroOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetList -> Buscando lista paginada com filtros: {@filter}", filter);

            var resultado = await _serviceBase.ExecutePaginatedQueryAsync<RelatorioModeloParametroOutputModel>(
                RelatorioModeloParametroProcedures.LST_TB_RELATORIO_MODELO_PARAMETRO,
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
    public async Task<NexusResult<RelatorioModeloParametroInputModel>> SalvarAsync(RelatorioModeloParametroInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Salvar -> Processando salvamento de relatoriomodeloparametro: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Salvar -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Salvar -> RelatorioModeloParametro salvo com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salvar -> Erro ao salvar relatoriomodeloparametro: {@model}", model);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloParametroInputModel>> ExcluirAsync(RelatorioModeloParametroInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Excluir -> Processando exclusão de relatoriomodeloparametro: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Excluir -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Excluir -> RelatorioModeloParametro excluído com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excluir -> Erro ao excluir relatoriomodeloparametro: {@model}", model);
            throw;
        }
    }

}
