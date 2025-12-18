using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioModeloPermissao
/// </summary>
public interface IRelatorioModeloPermissaoRepository
{
    /// <summary>
    /// Busca relatoriomodelopermissao por ID
    /// </summary>
    Task<NexusResult<RelatorioModeloPermissaoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelopermissaos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioModeloPermissaoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriomodelopermissao (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioModeloPermissaoInputModel>> SalvarAsync(RelatorioModeloPermissaoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriomodelopermissao
    /// </summary>
    Task<NexusResult<RelatorioModeloPermissaoInputModel>> ExcluirAsync(RelatorioModeloPermissaoInputModel model, CancellationToken cancellationToken = default);

}
