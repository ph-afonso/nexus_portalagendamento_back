using Microsoft.AspNetCore.Http;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

public class EnviarAnexoInputModel
{
    public Guid IdentificadorCliente { get; set; }
    public IFormFile Arquivo { get; set; }
}