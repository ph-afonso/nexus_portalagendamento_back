using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioModeloDestino
/// </summary>
public interface IRelatorioModeloDestinoService
{
    /// <summary>
    /// Obtém um relatoriomodelodestino pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriomodelodestino</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriomodelodestino encontrado ou null</returns>
    Task<RelatorioModeloDestinoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelodestinos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriomodelodestinos</returns>
    Task<PagedListNexusResult<RelatorioModeloDestinoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriomodelodestino
    /// </summary>
    /// <param name="relatoriomodelodestino">Dados do relatoriomodelodestino a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelodestino criado</returns>
    Task<NexusResult<RelatorioModeloDestinoInputModel>> CreateAsync(RelatorioModeloDestinoInputModel relatoriomodelodestino, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriomodelodestino existente
    /// </summary>
    /// <param name="relatoriomodelodestino">Dados do relatoriomodelodestino a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelodestino atualizado</returns>
    Task<NexusResult<RelatorioModeloDestinoInputModel>> UpdateAsync(RelatorioModeloDestinoInputModel relatoriomodelodestino, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriomodelodestino (exclusão)
    /// </summary>
    /// <param name="relatoriomodelodestino">Dados do relatoriomodelodestino a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioModeloDestinoInputModel>> DeleteAsync(RelatorioModeloDestinoInputModel relatoriomodelodestino, CancellationToken cancellationToken = default);

}
