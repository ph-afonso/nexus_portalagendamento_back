using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloSaida;

/// <summary>
/// Endpoint para buscar relatoriomodelosaida por ID
/// </summary>
public class GetRelatorioModeloSaidaByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioModeloSaida/{id:int}", HandleAsync)
            .WithName("GetRelatorioModeloSaidaById")
            .WithSummary("Busca um relatoriomodelosaida por ID")
            .WithDescription("Retorna os dados de um relatoriomodelosaida específico pelo seu ID")
            .WithTags("RelatorioModeloSaidas")
            .Produces<NexusResult<RelatorioModeloSaidaOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioModeloSaidaService relatoriomodelosaidaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloSaidaOutputModel>();

        try
        {
            var relatoriomodelosaida = await relatoriomodelosaidaService.GetByIdAsync(id, cancellationToken);

            if (relatoriomodelosaida == null)
            {
                result.AddFailureMessage($"RelatorioModeloSaida com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriomodelosaida;
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
