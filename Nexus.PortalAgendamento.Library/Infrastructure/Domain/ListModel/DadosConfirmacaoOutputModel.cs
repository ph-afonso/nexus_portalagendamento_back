namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class DadosConfirmacaoOutputModel
{
    public DateTime? DataSugestao { get; set; }
    public List<NotaFiscalOutputModel> NotasFiscais { get; set; } = new();
}