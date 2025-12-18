using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloPermissao;

/// <summary>
/// Endpoint para buscar relatoriomodelopermissao por ID
/// </summary>
public class GetRelatorioModeloPermissaoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/RelatorioModeloPermissao/{id:int}", HandleAsync)
            .WithName("GetRelatorioModeloPermissaoById")
            .WithSummary("Busca um relatoriomodelopermissao por ID")
            .WithDescription("Retorna os dados de um relatoriomodelopermissao específico pelo seu ID")
            .WithTags("RelatorioModeloPermissaos")
            .Produces<NexusResult<RelatorioModeloPermissaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IRelatorioModeloPermissaoService relatoriomodelopermissaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloPermissaoOutputModel>();

        try
        {
            var relatoriomodelopermissao = await relatoriomodelopermissaoService.GetByIdAsync(id, cancellationToken);

            if (relatoriomodelopermissao == null)
            {
                result.AddFailureMessage($"RelatorioModeloPermissao com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = relatoriomodelopermissao;
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
