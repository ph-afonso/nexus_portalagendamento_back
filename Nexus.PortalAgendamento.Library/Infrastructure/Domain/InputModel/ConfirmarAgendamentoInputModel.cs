namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
public class ConfirmarAgendamentoInputModel
{
    public Guid IdentificadorCliente { get; set; }
    // Se houver alguma observação na confirmação, adicione aqui.
    // Caso contrário, só o GUID basta para saber QUEM está confirmando.
}