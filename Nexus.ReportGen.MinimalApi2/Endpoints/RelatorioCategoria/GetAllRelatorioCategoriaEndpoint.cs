using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioCategoria;

/// <summary>
/// Endpoint para listar todos os relatoriocategorias com paginação
/// </summary>
public class GetAllRelatorioCategoriaEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioCategoria", HandleAsync)
            .WithName("GetAllRelatorioCategorias")
            .WithSummary("Lista todos os relatoriocategorias com paginação")
            .WithDescription("Retorna uma lista paginada de todos os relatoriocategorias")
            .WithTags("RelatorioCategorias")
            .Produces<PagedListNexusResult<RelatorioCategoriaOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromServices] IRelatorioCategoriaService relatoriocategoriaService,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
    {
        var result = new PagedListNexusResult<RelatorioCategoriaOutputModel>();

        try
        {
            var filter = new PagedFilterInputModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result = await relatoriocategoriaService.ListPaginadoAsync(filter, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }
}
