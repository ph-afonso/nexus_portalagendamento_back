using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioParametroOpcao;

/// <summary>
/// Endpoint para buscar relatorioparametroopcao por ID
/// </summary>
public class GetRelatorioParametroOpcaoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioParametroOpcao/{id:int}", HandleAsync)
            .WithName("GetRelatorioParametroOpcaoById")
            .WithSummary("Busca um relatorioparametroopcao por ID")
            .WithDescription("Retorna os dados de um relatorioparametroopcao específico pelo seu ID")
            .WithTags("RelatorioParametroOpcaos")
            .Produces<NexusResult<RelatorioParametroOpcaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioParametroOpcaoService relatorioparametroopcaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioParametroOpcaoOutputModel>();

        try
        {
            var relatorioparametroopcao = await relatorioparametroopcaoService.GetByIdAsync(id, cancellationToken);

            if (relatorioparametroopcao == null)
            {
                result.AddFailureMessage($"RelatorioParametroOpcao com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatorioparametroopcao;
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
