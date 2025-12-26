using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

public interface IPortalAgendamentoService
{
    Task<NexusResult<bool>> Confirmar(ConfirmarAgendamentoInputModel model, CancellationToken ct);
    Task<NexusResult<bool>> SolicitarNovaData(SolicitarNovaDataInputModel model, CancellationToken ct);
    Task<NexusResult<bool>> EnviarAnexo(EnviarAnexoInputModel model, CancellationToken ct);
}