using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioDestinoTipo;

/// <summary>
/// Endpoint para buscar relatoriodestinotipo por ID
/// </summary>
public class GetRelatorioDestinoTipoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioDestinoTipo/{id:int}", HandleAsync)
            .WithName("GetRelatorioDestinoTipoById")
            .WithSummary("Busca um relatoriodestinotipo por ID")
            .WithDescription("Retorna os dados de um relatoriodestinotipo específico pelo seu ID")
            .WithTags("RelatorioDestinoTipos")
            .Produces<NexusResult<RelatorioDestinoTipoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioDestinoTipoService relatoriodestinotipoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioDestinoTipoOutputModel>();

        try
        {
            var relatoriodestinotipo = await relatoriodestinotipoService.GetByIdAsync(id, cancellationToken);

            if (relatoriodestinotipo == null)
            {
                result.AddFailureMessage($"RelatorioDestinoTipo com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriodestinotipo;
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
