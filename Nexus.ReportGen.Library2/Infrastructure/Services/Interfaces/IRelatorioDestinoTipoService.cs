using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioDestinoTipo
/// </summary>
public interface IRelatorioDestinoTipoService
{
    /// <summary>
    /// Obtém um relatoriodestinotipo pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriodestinotipo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriodestinotipo encontrado ou null</returns>
    Task<RelatorioDestinoTipoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriodestinotipos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriodestinotipos</returns>
    Task<PagedListNexusResult<RelatorioDestinoTipoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriodestinotipo
    /// </summary>
    /// <param name="relatoriodestinotipo">Dados do relatoriodestinotipo a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriodestinotipo criado</returns>
    Task<NexusResult<RelatorioDestinoTipoInputModel>> CreateAsync(RelatorioDestinoTipoInputModel relatoriodestinotipo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriodestinotipo existente
    /// </summary>
    /// <param name="relatoriodestinotipo">Dados do relatoriodestinotipo a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriodestinotipo atualizado</returns>
    Task<NexusResult<RelatorioDestinoTipoInputModel>> UpdateAsync(RelatorioDestinoTipoInputModel relatoriodestinotipo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriodestinotipo (exclusão)
    /// </summary>
    /// <param name="relatoriodestinotipo">Dados do relatoriodestinotipo a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioDestinoTipoInputModel>> DeleteAsync(RelatorioDestinoTipoInputModel relatoriodestinotipo, CancellationToken cancellationToken = default);

}
