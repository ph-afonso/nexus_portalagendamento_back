using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de TipoParametro
/// </summary>
public interface ITipoParametroService
{
    /// <summary>
    /// Obtém um tipoparametro pelo ID
    /// </summary>
    /// <param name="id">ID do tipoparametro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O tipoparametro encontrado ou null</returns>
    Task<TipoParametroOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista tipoparametros com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de tipoparametros</returns>
    Task<PagedListNexusResult<TipoParametroOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo tipoparametro
    /// </summary>
    /// <param name="tipoparametro">Dados do tipoparametro a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o tipoparametro criado</returns>
    Task<NexusResult<TipoParametroInputModel>> CreateAsync(TipoParametroInputModel tipoparametro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um tipoparametro existente
    /// </summary>
    /// <param name="tipoparametro">Dados do tipoparametro a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o tipoparametro atualizado</returns>
    Task<NexusResult<TipoParametroInputModel>> UpdateAsync(TipoParametroInputModel tipoparametro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um tipoparametro (exclusão)
    /// </summary>
    /// <param name="tipoparametro">Dados do tipoparametro a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<TipoParametroInputModel>> DeleteAsync(TipoParametroInputModel tipoparametro, CancellationToken cancellationToken = default);

}
