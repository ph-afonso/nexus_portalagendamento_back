using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioModelo
/// </summary>
public interface IRelatorioModeloService
{
    /// <summary>
    /// Obtém um relatoriomodelo pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriomodelo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriomodelo encontrado ou null</returns>
    Task<RelatorioModeloOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriomodelos</returns>
    Task<PagedListNexusResult<RelatorioModeloOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriomodelo
    /// </summary>
    /// <param name="relatoriomodelo">Dados do relatoriomodelo a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelo criado</returns>
    Task<NexusResult<RelatorioModeloInputModel>> CreateAsync(RelatorioModeloInputModel relatoriomodelo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriomodelo existente
    /// </summary>
    /// <param name="relatoriomodelo">Dados do relatoriomodelo a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelo atualizado</returns>
    Task<NexusResult<RelatorioModeloInputModel>> UpdateAsync(RelatorioModeloInputModel relatoriomodelo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriomodelo (exclusão)
    /// </summary>
    /// <param name="relatoriomodelo">Dados do relatoriomodelo a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioModeloInputModel>> DeleteAsync(RelatorioModeloInputModel relatoriomodelo, CancellationToken cancellationToken = default);

}
