using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Services;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.Sample.MinimalApi.Endpoints.PortalAgendamento;

/// <summary>
/// Endpoint para consulta validade do Token
/// </summary>
public class GetDataAgendamentoConfirmacaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("portal-agendamento/data-agendamento-confirmacao/{identificadorCliente:guid}", HandleAsync)
            .WithName("GetDataAgendamentoConfirmacaoEndpoint")
            .WithSummary("Consultar data de agendamento")
            .WithDescription("Retorna data de agendamento confirmada")
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
            var dataAgendamento = await portalAgendamentoService.GetDataAgendamentoConfirmacao(identificadorCliente, cancellationToken);

            result.ResultData = dataAgendamento ?? new PortalAgendamentoOutputModel();
            if (result.ResultData.DataAgendamento is not null)
            {

                var dataAgendamentoFormatted = result.ResultData.DataAgendamento?.ToString("dd/MM/yyyy");
                result.ResultData.DataAgendamentoFormatted = dataAgendamentoFormatted;

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
