using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioModeloParametro
/// </summary>
public interface IRelatorioModeloParametroRepository
{
    /// <summary>
    /// Busca relatoriomodeloparametro por ID
    /// </summary>
    Task<NexusResult<RelatorioModeloParametroOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodeloparametros com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioModeloParametroOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriomodeloparametro (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioModeloParametroInputModel>> SalvarAsync(RelatorioModeloParametroInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriomodeloparametro
    /// </summary>
    Task<NexusResult<RelatorioModeloParametroInputModel>> ExcluirAsync(RelatorioModeloParametroInputModel model, CancellationToken cancellationToken = default);

}
