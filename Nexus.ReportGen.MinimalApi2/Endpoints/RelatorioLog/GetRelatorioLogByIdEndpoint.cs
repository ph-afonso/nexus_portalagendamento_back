using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioLog;

/// <summary>
/// Endpoint para buscar relatoriolog por ID
/// </summary>
public class GetRelatorioLogByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioLog/{id:int}", HandleAsync)
            .WithName("GetRelatorioLogById")
            .WithSummary("Busca um relatoriolog por ID")
            .WithDescription("Retorna os dados de um relatoriolog específico pelo seu ID")
            .WithTags("RelatorioLogs")
            .Produces<NexusResult<RelatorioLogOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioLogService relatoriologService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioLogOutputModel>();

        try
        {
            var relatoriolog = await relatoriologService.GetByIdAsync(id, cancellationToken);

            if (relatoriolog == null)
            {
                result.AddFailureMessage($"RelatorioLog com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriolog;
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
