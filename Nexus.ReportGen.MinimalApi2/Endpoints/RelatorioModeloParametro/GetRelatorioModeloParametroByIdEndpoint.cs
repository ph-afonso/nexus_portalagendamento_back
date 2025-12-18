using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloParametro;

/// <summary>
/// Endpoint para buscar relatoriomodeloparametro por ID
/// </summary>
public class GetRelatorioModeloParametroByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioModeloParametro/{id:int}", HandleAsync)
            .WithName("GetRelatorioModeloParametroById")
            .WithSummary("Busca um relatoriomodeloparametro por ID")
            .WithDescription("Retorna os dados de um relatoriomodeloparametro específico pelo seu ID")
            .WithTags("RelatorioModeloParametros")
            .Produces<NexusResult<RelatorioModeloParametroOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioModeloParametroService relatoriomodeloparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloParametroOutputModel>();

        try
        {
            var relatoriomodeloparametro = await relatoriomodeloparametroService.GetByIdAsync(id, cancellationToken);

            if (relatoriomodeloparametro == null)
            {
                result.AddFailureMessage($"RelatorioModeloParametro com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriomodeloparametro;
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
