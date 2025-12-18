using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioSolicitacao;

/// <summary>
/// Endpoint para buscar relatoriosolicitacao por ID
/// </summary>
public class GetRelatorioSolicitacaoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioSolicitacao/{id:int}", HandleAsync)
            .WithName("GetRelatorioSolicitacaoById")
            .WithSummary("Busca um relatoriosolicitacao por ID")
            .WithDescription("Retorna os dados de um relatoriosolicitacao específico pelo seu ID")
            .WithTags("RelatorioSolicitacaos")
            .Produces<NexusResult<RelatorioSolicitacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioSolicitacaoService relatoriosolicitacaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoOutputModel>();

        try
        {
            var relatoriosolicitacao = await relatoriosolicitacaoService.GetByIdAsync(id, cancellationToken);

            if (relatoriosolicitacao == null)
            {
                result.AddFailureMessage($"RelatorioSolicitacao com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriosolicitacao;
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
