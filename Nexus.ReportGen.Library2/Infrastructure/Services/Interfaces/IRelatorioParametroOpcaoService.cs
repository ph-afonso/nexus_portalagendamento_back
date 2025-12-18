using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioParametroOpcao
/// </summary>
public interface IRelatorioParametroOpcaoService
{
    /// <summary>
    /// Obtém um relatorioparametroopcao pelo ID
    /// </summary>
    /// <param name="id">ID do relatorioparametroopcao</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatorioparametroopcao encontrado ou null</returns>
    Task<RelatorioParametroOpcaoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatorioparametroopcaos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatorioparametroopcaos</returns>
    Task<PagedListNexusResult<RelatorioParametroOpcaoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatorioparametroopcao
    /// </summary>
    /// <param name="relatorioparametroopcao">Dados do relatorioparametroopcao a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatorioparametroopcao criado</returns>
    Task<NexusResult<RelatorioParametroOpcaoInputModel>> CreateAsync(RelatorioParametroOpcaoInputModel relatorioparametroopcao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatorioparametroopcao existente
    /// </summary>
    /// <param name="relatorioparametroopcao">Dados do relatorioparametroopcao a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatorioparametroopcao atualizado</returns>
    Task<NexusResult<RelatorioParametroOpcaoInputModel>> UpdateAsync(RelatorioParametroOpcaoInputModel relatorioparametroopcao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatorioparametroopcao (exclusão)
    /// </summary>
    /// <param name="relatorioparametroopcao">Dados do relatorioparametroopcao a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioParametroOpcaoInputModel>> DeleteAsync(RelatorioParametroOpcaoInputModel relatorioparametroopcao, CancellationToken cancellationToken = default);

}
