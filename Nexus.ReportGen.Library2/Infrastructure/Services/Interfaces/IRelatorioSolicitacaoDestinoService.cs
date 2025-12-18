using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioSolicitacaoDestino
/// </summary>
public interface IRelatorioSolicitacaoDestinoService
{
    /// <summary>
    /// Obtém um relatoriosolicitacaodestino pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriosolicitacaodestino</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriosolicitacaodestino encontrado ou null</returns>
    Task<RelatorioSolicitacaoDestinoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriosolicitacaodestinos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriosolicitacaodestinos</returns>
    Task<PagedListNexusResult<RelatorioSolicitacaoDestinoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriosolicitacaodestino
    /// </summary>
    /// <param name="relatoriosolicitacaodestino">Dados do relatoriosolicitacaodestino a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriosolicitacaodestino criado</returns>
    Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> CreateAsync(RelatorioSolicitacaoDestinoInputModel relatoriosolicitacaodestino, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriosolicitacaodestino existente
    /// </summary>
    /// <param name="relatoriosolicitacaodestino">Dados do relatoriosolicitacaodestino a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriosolicitacaodestino atualizado</returns>
    Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> UpdateAsync(RelatorioSolicitacaoDestinoInputModel relatoriosolicitacaodestino, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriosolicitacaodestino (exclusão)
    /// </summary>
    /// <param name="relatoriosolicitacaodestino">Dados do relatoriosolicitacaodestino a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> DeleteAsync(RelatorioSolicitacaoDestinoInputModel relatoriosolicitacaodestino, CancellationToken cancellationToken = default);

}
