using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.StatusProcessamento;

/// <summary>
/// Endpoint para listar todos os statusprocessamentos com paginação
/// </summary>
public class GetAllStatusProcessamentoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/StatusProcessamento", HandleAsync)
            .WithName("GetAllStatusProcessamentos")
            .WithSummary("Lista todos os statusprocessamentos com paginação")
            .WithDescription("Retorna uma lista paginada de todos os statusprocessamentos")
            .WithTags("StatusProcessamentos")
            .Produces<PagedListNexusResult<StatusProcessamentoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromServices] IStatusProcessamentoService statusprocessamentoService,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
    {
        var result = new PagedListNexusResult<StatusProcessamentoOutputModel>();

        try
        {
            var filter = new PagedFilterInputModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result = await statusprocessamentoService.ListPaginadoAsync(filter, cancellationToken);

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
