using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class NovaDataEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/nova-data", HandleAsync)
            .WithName("SolicitarNovaData")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromBody] SolicitarNovaDataInputModel model,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        var result = await service.SolicitarNovaData(model, ct);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}