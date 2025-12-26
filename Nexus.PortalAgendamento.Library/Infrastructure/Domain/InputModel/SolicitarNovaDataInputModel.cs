namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

public class SolicitarNovaDataInputModel
{
    public Guid IdentificadorCliente { get; set; }
    public DateTime NovaDataSugerida { get; set; }
    public string? Observacao { get; set; }
}