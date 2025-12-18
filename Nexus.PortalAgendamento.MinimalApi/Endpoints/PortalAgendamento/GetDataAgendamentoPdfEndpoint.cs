using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

/// <summary>
/// Endpoint para extrair data de agendamento do PDF
/// </summary>
public class GetDataAgendamentoPdfEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/data-agendamento-pdf", HandleAsync)
            .DisableAntiforgery()
            .WithName("GetDataAgendamentoPdf")
            .WithSummary("Extrair data de agendamento do PDF")
            .WithDescription("Retorna data de agendamento")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<PortalAgendamentoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromForm] IFormFile request,
        [FromServices] IPortalAgendamentoService portalAgendamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<PortalAgendamentoOutputModel>();

        try
        {

            if (request == null)
            {
                result.AddFailureMessage($"Dados do request são obrigatórios");
                return Results.NotFound(result);
            }

            var dataAgendamento = await portalAgendamentoService.GetDataAgendamentoPdf(request, cancellationToken);

            result.ResultData = dataAgendamento ?? new PortalAgendamentoOutputModel();
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
