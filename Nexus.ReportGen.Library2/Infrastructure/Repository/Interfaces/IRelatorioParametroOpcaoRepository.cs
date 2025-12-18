using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioParametroOpcao
/// </summary>
public interface IRelatorioParametroOpcaoRepository
{
    /// <summary>
    /// Busca relatorioparametroopcao por ID
    /// </summary>
    Task<NexusResult<RelatorioParametroOpcaoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatorioparametroopcaos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioParametroOpcaoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatorioparametroopcao (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioParametroOpcaoInputModel>> SalvarAsync(RelatorioParametroOpcaoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatorioparametroopcao
    /// </summary>
    Task<NexusResult<RelatorioParametroOpcaoInputModel>> ExcluirAsync(RelatorioParametroOpcaoInputModel model, CancellationToken cancellationToken = default);

}
