using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioModelo
/// </summary>
public interface IRelatorioModeloRepository
{
    /// <summary>
    /// Busca relatoriomodelo por ID
    /// </summary>
    Task<NexusResult<RelatorioModeloOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioModeloOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriomodelo (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioModeloInputModel>> SalvarAsync(RelatorioModeloInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriomodelo
    /// </summary>
    Task<NexusResult<RelatorioModeloInputModel>> ExcluirAsync(RelatorioModeloInputModel model, CancellationToken cancellationToken = default);

}
