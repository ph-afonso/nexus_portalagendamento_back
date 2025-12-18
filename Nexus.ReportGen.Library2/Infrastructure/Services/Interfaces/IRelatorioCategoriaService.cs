using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioCategoria
/// </summary>
public interface IRelatorioCategoriaService
{
    /// <summary>
    /// Obtém um relatoriocategoria pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriocategoria</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriocategoria encontrado ou null</returns>
    Task<RelatorioCategoriaOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriocategorias com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriocategorias</returns>
    Task<PagedListNexusResult<RelatorioCategoriaOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriocategoria
    /// </summary>
    /// <param name="relatoriocategoria">Dados do relatoriocategoria a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriocategoria criado</returns>
    Task<NexusResult<RelatorioCategoriaInputModel>> CreateAsync(RelatorioCategoriaInputModel relatoriocategoria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriocategoria existente
    /// </summary>
    /// <param name="relatoriocategoria">Dados do relatoriocategoria a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriocategoria atualizado</returns>
    Task<NexusResult<RelatorioCategoriaInputModel>> UpdateAsync(RelatorioCategoriaInputModel relatoriocategoria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriocategoria (exclusão)
    /// </summary>
    /// <param name="relatoriocategoria">Dados do relatoriocategoria a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioCategoriaInputModel>> DeleteAsync(RelatorioCategoriaInputModel relatoriocategoria, CancellationToken cancellationToken = default);

}
