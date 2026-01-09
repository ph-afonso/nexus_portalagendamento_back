namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

public class SolicitarAlteracaoInputModel
{
    public Guid IdentificadorCliente { get; set; }
    public DateTime DataSolicitada { get; set; }
    public string HoraSolicitada { get; set; } = "08:00"; // Ex: "08:00", "13:00"
    public int Periodo { get; set; } = 4;
}