using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloDestino;

/// <summary>
/// Endpoint para listar todos os relatoriomodelodestinos com paginação
/// </summary>
public class GetAllRelatorioModeloDestinoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioModeloDestino", HandleAsync)
            .WithName("GetAllRelatorioModeloDestinos")
            .WithSummary("Lista todos os relatoriomodelodestinos com paginação")
            .WithDescription("Retorna uma lista paginada de todos os relatoriomodelodestinos")
            .WithTags("RelatorioModeloDestinos")
            .Produces<PagedListNexusResult<RelatorioModeloDestinoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromServices] IRelatorioModeloDestinoService relatoriomodelodestinoService,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
    {
        var result = new PagedListNexusResult<RelatorioModeloDestinoOutputModel>();

        try
        {
            var filter = new PagedFilterInputModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result = await relatoriomodelodestinoService.ListPaginadoAsync(filter, cancellationToken);

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
