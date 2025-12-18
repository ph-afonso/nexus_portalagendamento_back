using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioSolicitacaoDestino;

/// <summary>
/// Endpoint para buscar relatoriosolicitacaodestino por ID
/// </summary>
public class GetRelatorioSolicitacaoDestinoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioSolicitacaoDestino/{id:int}", HandleAsync)
            .WithName("GetRelatorioSolicitacaoDestinoById")
            .WithSummary("Busca um relatoriosolicitacaodestino por ID")
            .WithDescription("Retorna os dados de um relatoriosolicitacaodestino específico pelo seu ID")
            .WithTags("RelatorioSolicitacaoDestinos")
            .Produces<NexusResult<RelatorioSolicitacaoDestinoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioSolicitacaoDestinoService relatoriosolicitacaodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoDestinoOutputModel>();

        try
        {
            var relatoriosolicitacaodestino = await relatoriosolicitacaodestinoService.GetByIdAsync(id, cancellationToken);

            if (relatoriosolicitacaodestino == null)
            {
                result.AddFailureMessage($"RelatorioSolicitacaoDestino com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriosolicitacaodestino;
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
