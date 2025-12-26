using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;

public interface IPortalAgendamentoRepository
{
    // Usado internamente para validar o token
    Task<NexusResult<dynamic>> GetDadosValidacaoToken(Guid identificadorCliente, CancellationToken ct);

    // As 3 ações principais
    Task<NexusResult<bool>> ConfirmarAgendamento(ConfirmarAgendamentoInputModel model, CancellationToken ct);
    Task<NexusResult<bool>> SolicitarNovaData(SolicitarNovaDataInputModel model, CancellationToken ct);
    Task<NexusResult<bool>> RegistrarAnexo(Guid identificadorCliente, string caminhoArquivo, CancellationToken ct);
}