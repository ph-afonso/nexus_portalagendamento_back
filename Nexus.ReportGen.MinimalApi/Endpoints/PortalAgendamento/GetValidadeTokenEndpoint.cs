using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Services;

namespace Nexus.Sample.MinimalApi.Endpoints.PortalAgendamento;

/// <summary>
/// Endpoint para consulta validade do Token
/// </summary>
public class GetValidadeTokenEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("portal-agendamento/validade/{identificadorCliente:guid}", HandleAsync)
            .WithName("GetValidadeToken")
            .WithSummary("Consultar validade token cliente")
            .WithDescription("Retorna a data de validade do token do cliente")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<PortalAgendamentoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid identificadorCliente,
        [FromServices] IPortalAgendamentoService portalAgendamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<PortalAgendamentoOutputModel>();

        try
        {
            var dataValidade = await portalAgendamentoService.GetValidadeToken(identificadorCliente, cancellationToken);

            result.ResultData = dataValidade ?? new PortalAgendamentoOutputModel();
            if (result.ResultData?.DataValidade is null)
            {
                result.ResultData.TokenValido = false;
            }
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }
}
