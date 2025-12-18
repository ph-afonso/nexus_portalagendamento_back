using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de StatusProcessamento
/// </summary>
public interface IStatusProcessamentoRepository
{
    /// <summary>
    /// Busca statusprocessamento por ID
    /// </summary>
    Task<NexusResult<StatusProcessamentoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista statusprocessamentos com paginação
    /// </summary>
    Task<PagedListNexusResult<StatusProcessamentoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva statusprocessamento (Create/Update)
    /// </summary>
    Task<NexusResult<StatusProcessamentoInputModel>> SalvarAsync(StatusProcessamentoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui statusprocessamento
    /// </summary>
    Task<NexusResult<StatusProcessamentoInputModel>> ExcluirAsync(StatusProcessamentoInputModel model, CancellationToken cancellationToken = default);

}
