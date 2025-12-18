using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.StatusProcessamento;

/// <summary>
/// Endpoint para buscar statusprocessamento por ID
/// </summary>
public class GetStatusProcessamentoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/StatusProcessamento/{id:int}", HandleAsync)
            .WithName("GetStatusProcessamentoById")
            .WithSummary("Busca um statusprocessamento por ID")
            .WithDescription("Retorna os dados de um statusprocessamento específico pelo seu ID")
            .WithTags("StatusProcessamentos")
            .Produces<NexusResult<StatusProcessamentoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IStatusProcessamentoService statusprocessamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<StatusProcessamentoOutputModel>();

        try
        {
            var statusprocessamento = await statusprocessamentoService.GetByIdAsync(id, cancellationToken);

            if (statusprocessamento == null)
            {
                result.AddFailureMessage($"StatusProcessamento com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = statusprocessamento;
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
