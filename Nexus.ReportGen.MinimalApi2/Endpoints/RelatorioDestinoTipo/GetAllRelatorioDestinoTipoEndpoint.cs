using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioDestinoTipo;

/// <summary>
/// Endpoint para listar todos os relatoriodestinotipos com paginação
/// </summary>
public class GetAllRelatorioDestinoTipoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioDestinoTipo", HandleAsync)
            .WithName("GetAllRelatorioDestinoTipos")
            .WithSummary("Lista todos os relatoriodestinotipos com paginação")
            .WithDescription("Retorna uma lista paginada de todos os relatoriodestinotipos")
            .WithTags("RelatorioDestinoTipos")
            .Produces<PagedListNexusResult<RelatorioDestinoTipoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromServices] IRelatorioDestinoTipoService relatoriodestinotipoService,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
    {
        var result = new PagedListNexusResult<RelatorioDestinoTipoOutputModel>();

        try
        {
            var filter = new PagedFilterInputModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result = await relatoriodestinotipoService.ListPaginadoAsync(filter, cancellationToken);

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
