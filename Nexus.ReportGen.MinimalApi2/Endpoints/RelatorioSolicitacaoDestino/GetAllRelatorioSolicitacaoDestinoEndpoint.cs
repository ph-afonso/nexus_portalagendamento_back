using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioSolicitacaoDestino;

/// <summary>
/// Endpoint para listar todos os relatoriosolicitacaodestinos com paginação
/// </summary>
public class GetAllRelatorioSolicitacaoDestinoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioSolicitacaoDestino", HandleAsync)
            .WithName("GetAllRelatorioSolicitacaoDestinos")
            .WithSummary("Lista todos os relatoriosolicitacaodestinos com paginação")
            .WithDescription("Retorna uma lista paginada de todos os relatoriosolicitacaodestinos")
            .WithTags("RelatorioSolicitacaoDestinos")
            .Produces<PagedListNexusResult<RelatorioSolicitacaoDestinoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromServices] IRelatorioSolicitacaoDestinoService relatoriosolicitacaodestinoService,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
    {
        var result = new PagedListNexusResult<RelatorioSolicitacaoDestinoOutputModel>();

        try
        {
            var filter = new PagedFilterInputModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result = await relatoriosolicitacaodestinoService.ListPaginadoAsync(filter, cancellationToken);

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
