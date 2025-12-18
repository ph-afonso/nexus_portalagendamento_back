using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioLog
/// </summary>
public interface IRelatorioLogRepository
{
    /// <summary>
    /// Busca relatoriolog por ID
    /// </summary>
    Task<NexusResult<RelatorioLogOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriologs com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioLogOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriolog (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioLogInputModel>> SalvarAsync(RelatorioLogInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriolog
    /// </summary>
    Task<NexusResult<RelatorioLogInputModel>> ExcluirAsync(RelatorioLogInputModel model, CancellationToken cancellationToken = default);

}
