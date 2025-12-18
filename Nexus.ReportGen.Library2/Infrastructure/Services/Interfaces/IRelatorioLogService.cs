using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioLog
/// </summary>
public interface IRelatorioLogService
{
    /// <summary>
    /// Obtém um relatoriolog pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriolog</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriolog encontrado ou null</returns>
    Task<RelatorioLogOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriologs com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriologs</returns>
    Task<PagedListNexusResult<RelatorioLogOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriolog
    /// </summary>
    /// <param name="relatoriolog">Dados do relatoriolog a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriolog criado</returns>
    Task<NexusResult<RelatorioLogInputModel>> CreateAsync(RelatorioLogInputModel relatoriolog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriolog existente
    /// </summary>
    /// <param name="relatoriolog">Dados do relatoriolog a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriolog atualizado</returns>
    Task<NexusResult<RelatorioLogInputModel>> UpdateAsync(RelatorioLogInputModel relatoriolog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriolog (exclusão)
    /// </summary>
    /// <param name="relatoriolog">Dados do relatoriolog a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioLogInputModel>> DeleteAsync(RelatorioLogInputModel relatoriolog, CancellationToken cancellationToken = default);

}
