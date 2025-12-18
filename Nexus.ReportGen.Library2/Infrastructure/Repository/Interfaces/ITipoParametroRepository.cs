using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de TipoParametro
/// </summary>
public interface ITipoParametroRepository
{
    /// <summary>
    /// Busca tipoparametro por ID
    /// </summary>
    Task<NexusResult<TipoParametroOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista tipoparametros com paginação
    /// </summary>
    Task<PagedListNexusResult<TipoParametroOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva tipoparametro (Create/Update)
    /// </summary>
    Task<NexusResult<TipoParametroInputModel>> SalvarAsync(TipoParametroInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui tipoparametro
    /// </summary>
    Task<NexusResult<TipoParametroInputModel>> ExcluirAsync(TipoParametroInputModel model, CancellationToken cancellationToken = default);

}
