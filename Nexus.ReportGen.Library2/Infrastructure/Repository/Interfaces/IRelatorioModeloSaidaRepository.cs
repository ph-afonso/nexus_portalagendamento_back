using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioModeloSaida
/// </summary>
public interface IRelatorioModeloSaidaRepository
{
    /// <summary>
    /// Busca relatoriomodelosaida por ID
    /// </summary>
    Task<NexusResult<RelatorioModeloSaidaOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelosaidas com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioModeloSaidaOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriomodelosaida (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioModeloSaidaInputModel>> SalvarAsync(RelatorioModeloSaidaInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriomodelosaida
    /// </summary>
    Task<NexusResult<RelatorioModeloSaidaInputModel>> ExcluirAsync(RelatorioModeloSaidaInputModel model, CancellationToken cancellationToken = default);

}
