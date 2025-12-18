using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioModeloSaida
/// </summary>
public interface IRelatorioModeloSaidaService
{
    /// <summary>
    /// Obtém um relatoriomodelosaida pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriomodelosaida</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriomodelosaida encontrado ou null</returns>
    Task<RelatorioModeloSaidaOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelosaidas com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriomodelosaidas</returns>
    Task<PagedListNexusResult<RelatorioModeloSaidaOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriomodelosaida
    /// </summary>
    /// <param name="relatoriomodelosaida">Dados do relatoriomodelosaida a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelosaida criado</returns>
    Task<NexusResult<RelatorioModeloSaidaInputModel>> CreateAsync(RelatorioModeloSaidaInputModel relatoriomodelosaida, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriomodelosaida existente
    /// </summary>
    /// <param name="relatoriomodelosaida">Dados do relatoriomodelosaida a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelosaida atualizado</returns>
    Task<NexusResult<RelatorioModeloSaidaInputModel>> UpdateAsync(RelatorioModeloSaidaInputModel relatoriomodelosaida, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriomodelosaida (exclusão)
    /// </summary>
    /// <param name="relatoriomodelosaida">Dados do relatoriomodelosaida a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioModeloSaidaInputModel>> DeleteAsync(RelatorioModeloSaidaInputModel relatoriomodelosaida, CancellationToken cancellationToken = default);

}
