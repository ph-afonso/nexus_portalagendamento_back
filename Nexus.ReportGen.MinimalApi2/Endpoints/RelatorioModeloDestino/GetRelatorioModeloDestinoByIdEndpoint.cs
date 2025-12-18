using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloDestino;

/// <summary>
/// Endpoint para buscar relatoriomodelodestino por ID
/// </summary>
public class GetRelatorioModeloDestinoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioModeloDestino/{id:int}", HandleAsync)
            .WithName("GetRelatorioModeloDestinoById")
            .WithSummary("Busca um relatoriomodelodestino por ID")
            .WithDescription("Retorna os dados de um relatoriomodelodestino específico pelo seu ID")
            .WithTags("RelatorioModeloDestinos")
            .Produces<NexusResult<RelatorioModeloDestinoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioModeloDestinoService relatoriomodelodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloDestinoOutputModel>();

        try
        {
            var relatoriomodelodestino = await relatoriomodelodestinoService.GetByIdAsync(id, cancellationToken);

            if (relatoriomodelodestino == null)
            {
                result.AddFailureMessage($"RelatorioModeloDestino com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriomodelodestino;
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
