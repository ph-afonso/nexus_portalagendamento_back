using Microsoft.AspNetCore.Http;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

public interface IPortalAgendamentoService
{
    // Métodos Básicos
    Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken ct);
    Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken ct);

    // Core de Agendamento
    Task<NexusResult<List<AgendamentoDetalheModel>>> ConfirmarAgendamento(
        Guid identificadorCliente,
        DateTime dataAgendamento,
        List<NotaFiscalOutputModel> notas,
        CancellationToken ct = default);

    // --- MÉTODOS DE ANEXO ---

    /// <summary>
    /// Recebe o arquivo, salva temporariamente e retorna listas separadas de Datas e Horas encontradas.
    /// </summary>
    Task<NexusResult<AnaliseAnexoOutputModel>> UploadAnaliseAnexoAsync(Guid identificadorCliente, IFormFile arquivo, CancellationToken ct);

    /// <summary>
    /// Realiza o agendamento buscando o arquivo previamente salvo na pasta temporária.
    /// </summary>
    Task<NexusResult<ConfirmacaoOutputModel>> AgendarComAnexoTempAsync(ConfirmacaoInputModel input, DateTime dataSolicitada, CancellationToken ct);
}