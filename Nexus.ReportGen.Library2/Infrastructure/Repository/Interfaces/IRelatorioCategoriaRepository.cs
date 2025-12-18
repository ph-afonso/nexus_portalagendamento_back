using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioCategoria
/// </summary>
public interface IRelatorioCategoriaRepository
{
    /// <summary>
    /// Busca relatoriocategoria por ID
    /// </summary>
    Task<NexusResult<RelatorioCategoriaOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriocategorias com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioCategoriaOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriocategoria (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioCategoriaInputModel>> SalvarAsync(RelatorioCategoriaInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriocategoria
    /// </summary>
    Task<NexusResult<RelatorioCategoriaInputModel>> ExcluirAsync(RelatorioCategoriaInputModel model, CancellationToken cancellationToken = default);

}
