using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class ConfirmarEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/confirmar", HandleAsync)
            .WithName("ConfirmarAgendamento")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfirmarAgendamentoInputModel model,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        var result = await service.Confirmar(model, ct);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}