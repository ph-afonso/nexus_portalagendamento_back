using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioDestinoTipo
/// </summary>
public interface IRelatorioDestinoTipoRepository
{
    /// <summary>
    /// Busca relatoriodestinotipo por ID
    /// </summary>
    Task<NexusResult<RelatorioDestinoTipoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriodestinotipos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioDestinoTipoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriodestinotipo (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioDestinoTipoInputModel>> SalvarAsync(RelatorioDestinoTipoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriodestinotipo
    /// </summary>
    Task<NexusResult<RelatorioDestinoTipoInputModel>> ExcluirAsync(RelatorioDestinoTipoInputModel model, CancellationToken cancellationToken = default);

}
