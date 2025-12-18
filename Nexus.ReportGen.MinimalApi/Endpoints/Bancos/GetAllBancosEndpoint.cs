using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;

namespace Nexus.Sample.MinimalApi.Endpoints.Bancos;

/// <summary>
/// Endpoint para listar todos os bancoss com paginação
/// </summary>
public class GetAllBancosEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/Bancos", HandleAsync)
            .WithName("GetAllBancoss")
            .WithSummary("Lista todos os bancoss com paginação")
            .WithDescription("Retorna uma lista paginada de todos os bancoss")
            .WithTags("Bancoss")
            .Produces<PagedListNexusResult<BancosOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromServices] IBancosService bancosService,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
    {
        var result = new PagedListNexusResult<BancosOutputModel>();

        try
        {
            var filter = new PagedFilterInputModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result = await bancosService.ListPaginadoAsync(filter, cancellationToken);

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
