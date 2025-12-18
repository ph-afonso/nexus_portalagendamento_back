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
/// Implementação do repositório de RelatorioSolicitacaoDestino usando ProcedureRepository
/// </summary>
public class RelatorioSolicitacaoDestinoRepository : ProcedureRepository, IRelatorioSolicitacaoDestinoRepository
{
    private readonly IServiceBase _serviceBase;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioSolicitacaoDestinoRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoDestinoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetByRelatorioSolicitacaoDestino -> Buscando relatoriosolicitacaodestino para ID: {id}", id);

            var result = await _serviceBase.FindByNumericIdAsync<RelatorioSolicitacaoDestinoOutputModel>(
                RelatorioSolicitacaoDestinoProcedures.SNG_TB_RELATORIO_SOLICITACAO_DESTINO,
                "COD_SOLICITACAO_DESTINO",
                id);

            _logger.LogInformation("GetByRelatorioSolicitacaoDestino -> Resultado obtido com sucesso para ID: {id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByRelatorioSolicitacaoDestino -> Erro ao buscar relatoriosolicitacaodestino para ID: {id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioSolicitacaoDestinoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetList -> Buscando lista paginada com filtros: {@filter}", filter);

            var resultado = await _serviceBase.ExecutePaginatedQueryAsync<RelatorioSolicitacaoDestinoOutputModel>(
                RelatorioSolicitacaoDestinoProcedures.LST_TB_RELATORIO_SOLICITACAO_DESTINO,
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
    public async Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> SalvarAsync(RelatorioSolicitacaoDestinoInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Salvar -> Processando salvamento de relatoriosolicitacaodestino: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Salvar -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Salvar -> RelatorioSolicitacaoDestino salvo com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salvar -> Erro ao salvar relatoriosolicitacaodestino: {@model}", model);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> ExcluirAsync(RelatorioSolicitacaoDestinoInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Excluir -> Processando exclusão de relatoriosolicitacaodestino: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Excluir -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var resultado = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Excluir -> RelatorioSolicitacaoDestino excluído com sucesso");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excluir -> Erro ao excluir relatoriosolicitacaodestino: {@model}", model);
            throw;
        }
    }

}
