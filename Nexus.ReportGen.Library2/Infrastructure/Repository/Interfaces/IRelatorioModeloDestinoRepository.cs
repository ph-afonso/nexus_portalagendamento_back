using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioModeloDestino
/// </summary>
public interface IRelatorioModeloDestinoRepository
{
    /// <summary>
    /// Busca relatoriomodelodestino por ID
    /// </summary>
    Task<NexusResult<RelatorioModeloDestinoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelodestinos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioModeloDestinoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriomodelodestino (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioModeloDestinoInputModel>> SalvarAsync(RelatorioModeloDestinoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriomodelodestino
    /// </summary>
    Task<NexusResult<RelatorioModeloDestinoInputModel>> ExcluirAsync(RelatorioModeloDestinoInputModel model, CancellationToken cancellationToken = default);

}
