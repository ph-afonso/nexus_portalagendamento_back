using Microsoft.AspNetCore.Http;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

public interface IPortalAgendamentoService
{
    Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken ct);
    Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken ct);

    // Core de Agendamento
    Task<NexusResult<List<AgendamentoDetalheModel>>> ConfirmarAgendamento(
        Guid identificadorCliente,
        DateTime dataAgendamento,
        List<NotaFiscalOutputModel> notas,
        CancellationToken ct = default);
    Task<NexusResult<AnaliseAnexoOutputModel>> UploadAnaliseAnexoAsync(Guid identificadorCliente, IFormFile arquivo, CancellationToken ct);
    Task<NexusResult<ConfirmacaoOutputModel>> AgendarComAnexoTempAsync(ConfirmacaoInputModel input, DateTime dataSolicitada, CancellationToken ct);
}