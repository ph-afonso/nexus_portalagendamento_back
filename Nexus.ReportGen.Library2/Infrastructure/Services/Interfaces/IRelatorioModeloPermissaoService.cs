using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioModeloPermissao
/// </summary>
public interface IRelatorioModeloPermissaoService
{
    /// <summary>
    /// Obtém um relatoriomodelopermissao pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriomodelopermissao</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriomodelopermissao encontrado ou null</returns>
    Task<RelatorioModeloPermissaoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodelopermissaos com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriomodelopermissaos</returns>
    Task<PagedListNexusResult<RelatorioModeloPermissaoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriomodelopermissao
    /// </summary>
    /// <param name="relatoriomodelopermissao">Dados do relatoriomodelopermissao a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelopermissao criado</returns>
    Task<NexusResult<RelatorioModeloPermissaoInputModel>> CreateAsync(RelatorioModeloPermissaoInputModel relatoriomodelopermissao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriomodelopermissao existente
    /// </summary>
    /// <param name="relatoriomodelopermissao">Dados do relatoriomodelopermissao a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodelopermissao atualizado</returns>
    Task<NexusResult<RelatorioModeloPermissaoInputModel>> UpdateAsync(RelatorioModeloPermissaoInputModel relatoriomodelopermissao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriomodelopermissao (exclusão)
    /// </summary>
    /// <param name="relatoriomodelopermissao">Dados do relatoriomodelopermissao a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioModeloPermissaoInputModel>> DeleteAsync(RelatorioModeloPermissaoInputModel relatoriomodelopermissao, CancellationToken cancellationToken = default);

}
