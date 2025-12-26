using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class AnexoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/anexo", HandleAsync)
            .WithName("EnviarAnexo")
            .WithTags("PortalAgendamento")
            .DisableAntiforgery() // Necessário para upload
            .Produces<NexusResult<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        Guid identificadorCliente, // Recebe via Query String ou Form
        IFormFile arquivo,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        var model = new EnviarAnexoInputModel
        {
            IdentificadorCliente = identificadorCliente,
            Arquivo = arquivo
        };

        var result = await service.EnviarAnexo(model, ct);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}