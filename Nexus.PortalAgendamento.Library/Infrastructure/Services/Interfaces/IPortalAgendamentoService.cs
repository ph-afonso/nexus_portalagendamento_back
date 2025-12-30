using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

public interface IPortalAgendamentoService
{
    // Valida Token
    Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken cancellationToken = default);

    // Busca Notas
    Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    // Processo Principal
    Task<NexusResult<List<AgendamentoDetalheModel>>> ConfirmarAgendamento(Guid identificadorCliente, DateTime dataAgendamento, List<NotaFiscalOutputModel> notas, CancellationToken cancellationToken = default);
}