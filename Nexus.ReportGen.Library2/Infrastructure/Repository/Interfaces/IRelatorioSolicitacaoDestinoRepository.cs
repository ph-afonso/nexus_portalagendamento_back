using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de RelatorioSolicitacaoDestino
/// </summary>
public interface IRelatorioSolicitacaoDestinoRepository
{
    /// <summary>
    /// Busca relatoriosolicitacaodestino por ID
    /// </summary>
    Task<NexusResult<RelatorioSolicitacaoDestinoOutputModel>> GetByAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista relatoriosolicitacaodestinos com paginação
    /// </summary>
    Task<PagedListNexusResult<RelatorioSolicitacaoDestinoOutputModel>> GetListAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva relatoriosolicitacaodestino (Create/Update)
    /// </summary>
    Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> SalvarAsync(RelatorioSolicitacaoDestinoInputModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui relatoriosolicitacaodestino
    /// </summary>
    Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> ExcluirAsync(RelatorioSolicitacaoDestinoInputModel model, CancellationToken cancellationToken = default);

}
