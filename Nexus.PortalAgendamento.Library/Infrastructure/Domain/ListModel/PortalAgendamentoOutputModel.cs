using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class PortalAgendamentoOutputModel
{
    public DateTime? DataAgendamento { get; set; }

    public List<DateTime> DataAgendamentoList { get; set; } = new List<DateTime>();

    public string? DataAgendamentoFormatted { get; set; }
    public DateTime? DataValidade { get; set; }
    public List<NotaFiscalOutputModel>? NotasFiscais { get; set; }
    public bool TokenValido { get; set; }
}