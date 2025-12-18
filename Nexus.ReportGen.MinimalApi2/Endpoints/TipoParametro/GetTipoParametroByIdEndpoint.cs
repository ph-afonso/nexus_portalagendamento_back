using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.TipoParametro;

/// <summary>
/// Endpoint para buscar tipoparametro por ID
/// </summary>
public class GetTipoParametroByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/TipoParametro/{id:int}", HandleAsync)
            .WithName("GetTipoParametroById")
            .WithSummary("Busca um tipoparametro por ID")
            .WithDescription("Retorna os dados de um tipoparametro específico pelo seu ID")
            .WithTags("TipoParametros")
            .Produces<NexusResult<TipoParametroOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] ITipoParametroService tipoparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<TipoParametroOutputModel>();

        try
        {
            var tipoparametro = await tipoparametroService.GetByIdAsync(id, cancellationToken);

            if (tipoparametro == null)
            {
                result.AddFailureMessage($"TipoParametro com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = tipoparametro;
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
