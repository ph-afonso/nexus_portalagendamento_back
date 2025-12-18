using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de RelatorioModeloParametro
/// </summary>
public interface IRelatorioModeloParametroService
{
    /// <summary>
    /// Obtém um relatoriomodeloparametro pelo ID
    /// </summary>
    /// <param name="id">ID do relatoriomodeloparametro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O relatoriomodeloparametro encontrado ou null</returns>
    Task<RelatorioModeloParametroOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriomodeloparametros com paginação
    /// </summary>
    /// <param name="filter">Filtros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de relatoriomodeloparametros</returns>
    Task<PagedListNexusResult<RelatorioModeloParametroOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo relatoriomodeloparametro
    /// </summary>
    /// <param name="relatoriomodeloparametro">Dados do relatoriomodeloparametro a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodeloparametro criado</returns>
    Task<NexusResult<RelatorioModeloParametroInputModel>> CreateAsync(RelatorioModeloParametroInputModel relatoriomodeloparametro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um relatoriomodeloparametro existente
    /// </summary>
    /// <param name="relatoriomodeloparametro">Dados do relatoriomodeloparametro a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o relatoriomodeloparametro atualizado</returns>
    Task<NexusResult<RelatorioModeloParametroInputModel>> UpdateAsync(RelatorioModeloParametroInputModel relatoriomodeloparametro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um relatoriomodeloparametro (exclusão)
    /// </summary>
    /// <param name="relatoriomodeloparametro">Dados do relatoriomodeloparametro a ser removido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<NexusResult<RelatorioModeloParametroInputModel>> DeleteAsync(RelatorioModeloParametroInputModel relatoriomodeloparametro, CancellationToken cancellationToken = default);

}
