using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;

namespace Nexus.Sample.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de Bancos
/// </summary>
public interface IBancosRepository
{
    /// <summary>
    /// Busca bancos por ID
    /// </summary>
    Task<NexusResult<BancosOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista bancoss com paginação
    /// </summary>
    Task<PagedListNexusResult<BancosOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva bancos (Create/Update)
    /// </summary>
    Task<NexusResult<BancosInputModel>> SalvarAsync(BancosInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui bancos
    /// </summary>
    Task<NexusResult<BancosInputModel>> ExcluirAsync(BancosInputModel model, CancellationToken cancellationToken = default);

}
