using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModelo;

/// <summary>
/// Endpoint para buscar relatoriomodelo por ID
/// </summary>
public class GetRelatorioModeloByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioModelo/{id:int}", HandleAsync)
            .WithName("GetRelatorioModeloById")
            .WithSummary("Busca um relatoriomodelo por ID")
            .WithDescription("Retorna os dados de um relatoriomodelo específico pelo seu ID")
            .WithTags("RelatorioModelos")
            .Produces<NexusResult<RelatorioModeloOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioModeloService relatoriomodeloService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloOutputModel>();

        try
        {
            var relatoriomodelo = await relatoriomodeloService.GetByIdAsync(id, cancellationToken);

            if (relatoriomodelo == null)
            {
                result.AddFailureMessage($"RelatorioModelo com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriomodelo;
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
