using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Sample.Library.Infrastructure.Domain;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;

namespace Nexus.Sample.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de Bancos
/// </summary>
public interface IBancosService
{
    /// <summary>
    /// Obtém um bancos pelo ID
    /// </summary>
    /// <param name="id">ID do bancos</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O bancos encontrado ou null</returns>
    Task<BancosOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista bancoss com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de bancoss</returns>
    Task<PagedListNexusResult<BancosOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo bancos
    /// </summary>
    /// <param name="bancos">Dados do bancos a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o bancos criado</returns>
    Task<NexusResult<BancosInputModel>> CreateAsync(BancosInputModel bancos, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um bancos existente
    /// </summary>
    /// <param name="bancos">Dados do bancos a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o bancos atualizado</returns>
    Task<NexusResult<BancosInputModel>> UpdateAsync(BancosInputModel bancos, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um bancos (exclusão)
    /// </summary>
    /// <param name="bancos">Dados do bancos a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<BancosInputModel>> DeleteAsync(BancosInputModel bancos, CancellationToken cancellationToken = default);

}
