using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioSolicitacao
/// </summary>
public interface IRelatorioSolicitacaoRepository
{
    /// <summary>
    /// Busca relatoriosolicitacao por ID
    /// </summary>
    Task<NexusResult<RelatorioSolicitacaoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriosolicitacaos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioSolicitacaoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriosolicitacao (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioSolicitacaoInputModel>> SalvarAsync(RelatorioSolicitacaoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriosolicitacao
    /// </summary>
    Task<NexusResult<RelatorioSolicitacaoInputModel>> ExcluirAsync(RelatorioSolicitacaoInputModel model, CancellationToken cancellationToken = default);

}
