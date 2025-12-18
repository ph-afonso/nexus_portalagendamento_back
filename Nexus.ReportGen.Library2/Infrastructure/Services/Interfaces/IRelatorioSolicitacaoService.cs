using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioSolicitacao
/// </summary>
public interface IRelatorioSolicitacaoService
{
    /// <summary>
    /// Obtém um relatoriosolicitacao pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriosolicitacao</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriosolicitacao encontrado ou null</returns>
    Task<RelatorioSolicitacaoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriosolicitacaos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriosolicitacaos</returns>
    Task<PagedListNexusResult<RelatorioSolicitacaoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriosolicitacao
    /// </summary>
    /// <param name="relatoriosolicitacao">Dados do relatoriosolicitacao a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriosolicitacao criado</returns>
    Task<NexusResult<RelatorioSolicitacaoInputModel>> CreateAsync(RelatorioSolicitacaoInputModel relatoriosolicitacao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriosolicitacao existente
    /// </summary>
    /// <param name="relatoriosolicitacao">Dados do relatoriosolicitacao a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriosolicitacao atualizado</returns>
    Task<NexusResult<RelatorioSolicitacaoInputModel>> UpdateAsync(RelatorioSolicitacaoInputModel relatoriosolicitacao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriosolicitacao (exclusão)
    /// </summary>
    /// <param name="relatoriosolicitacao">Dados do relatoriosolicitacao a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioSolicitacaoInputModel>> DeleteAsync(RelatorioSolicitacaoInputModel relatoriosolicitacao, CancellationToken cancellationToken = default);

}
