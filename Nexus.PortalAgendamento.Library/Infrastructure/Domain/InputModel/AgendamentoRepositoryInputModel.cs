using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

public class AgendamentoRepositoryInputModel
{
    public Guid IdentificadorCliente { get; set; }
    public DateTime DataAgendamento { get; set; }
    public List<NotaFiscalOutputModel> Notas { get; set; } = new();
}