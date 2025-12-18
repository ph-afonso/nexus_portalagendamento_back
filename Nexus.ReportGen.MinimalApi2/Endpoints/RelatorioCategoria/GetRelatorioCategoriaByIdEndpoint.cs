using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioCategoria;

/// <summary>
/// Endpoint para buscar relatoriocategoria por ID
/// </summary>
public class GetRelatorioCategoriaByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioCategoria/{id:int}", HandleAsync)
            .WithName("GetRelatorioCategoriaById")
            .WithSummary("Busca um relatoriocategoria por ID")
            .WithDescription("Retorna os dados de um relatoriocategoria específico pelo seu ID")
            .WithTags("RelatorioCategorias")
            .Produces<NexusResult<RelatorioCategoriaOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioCategoriaService relatoriocategoriaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioCategoriaOutputModel>();

        try
        {
            var relatoriocategoria = await relatoriocategoriaService.GetByIdAsync(id, cancellationToken);

            if (relatoriocategoria == null)
            {
                result.AddFailureMessage($"RelatorioCategoria com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriocategoria;
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
