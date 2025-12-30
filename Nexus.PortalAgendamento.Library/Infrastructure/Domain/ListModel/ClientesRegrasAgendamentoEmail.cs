namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class ClientesRegrasAgendamentoEmail
{
    // O Dapper mapeia pelo nome da coluna retornada na procedure.
    // Ajuste "EMAIL" se a coluna na procedure tiver outro nome (ex: EMAIL_CONTATO, DS_EMAIL)
    public string? Email { get; set; }
    public string? Nome { get; set; }
}