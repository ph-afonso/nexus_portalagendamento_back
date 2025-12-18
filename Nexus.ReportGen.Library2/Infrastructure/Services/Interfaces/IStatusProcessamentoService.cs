using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de StatusProcessamento
/// </summary>
public interface IStatusProcessamentoService
{
    /// <summary>
    /// Obtém um statusprocessamento pelo ID
    /// </summary>
    /// <param name="id">ID do statusprocessamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O statusprocessamento encontrado ou null</returns>
    Task<StatusProcessamentoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista statusprocessamentos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de statusprocessamentos</returns>
    Task<PagedListNexusResult<StatusProcessamentoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo statusprocessamento
    /// </summary>
    /// <param name="statusprocessamento">Dados do statusprocessamento a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o statusprocessamento criado</returns>
    Task<NexusResult<StatusProcessamentoInputModel>> CreateAsync(StatusProcessamentoInputModel statusprocessamento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um statusprocessamento existente
    /// </summary>
    /// <param name="statusprocessamento">Dados do statusprocessamento a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o statusprocessamento atualizado</returns>
    Task<NexusResult<StatusProcessamentoInputModel>> UpdateAsync(StatusProcessamentoInputModel statusprocessamento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um statusprocessamento (exclusão)
    /// </summary>
    /// <param name="statusprocessamento">Dados do statusprocessamento a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<StatusProcessamentoInputModel>> DeleteAsync(StatusProcessamentoInputModel statusprocessamento, CancellationToken cancellationToken = default);

}
