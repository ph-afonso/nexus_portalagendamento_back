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
/// Implementação do repositório de RelatorioLog usando ProcedureRepository
/// </summary>
public class RelatorioLogRepository : ProcedureRepository, IRelatorioLogRepository
{
    private readonly IServiceBase _serviceBase;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioLogRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioLogOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetByRelatorioLog -> Buscando relatoriolog para ID: {id}", id);

            var result = await _serviceBase.FindByNumericIdAsync<RelatorioLogOutputModel>(
                RelatorioLogProcedures.SNG_TB_RELATORIO_LOG,
                "COD_LOG",
                id);

            _logger.LogInformation("GetByRelatorioLog -> Resultado obtido com sucesso para ID: {id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByRelatorioLog -> Erro ao buscar relatoriolog para ID: {id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioLogOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetList -> Buscando lista paginada com filtros: {@filter}", filter);

            var resultado = await _serviceBase.ExecutePaginatedQueryAsync<RelatorioLogOutputModel>(
                RelatorioLogProcedures.LST_TB_RELATORIO_LOG,
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
    public async Task<NexusResult<RelatorioLogInputModel>> SalvarAsync(RelatorioLogInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Salvar -> Processando salvamento de relatoriolog: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Salvar -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Salvar -> RelatorioLog salvo com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salvar -> Erro ao salvar relatoriolog: {@model}", model);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioLogInputModel>> ExcluirAsync(RelatorioLogInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Excluir -> Processando exclusão de relatoriolog: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Excluir -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Excluir -> RelatorioLog excluído com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excluir -> Erro ao excluir relatoriolog: {@model}", model);
            throw;
        }
    }

}
