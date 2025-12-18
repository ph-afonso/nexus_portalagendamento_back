using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;

namespace Nexus.Sample.MinimalApi.Endpoints.Bancos;

/// <summary>
/// Endpoint para buscar bancos por ID
/// </summary>
public class GetBancosByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/Bancos/{id:int}", HandleAsync)
            .WithName("GetBancosById")
            .WithSummary("Busca um bancos por ID")
            .WithDescription("Retorna os dados de um bancos específico pelo seu ID")
            .WithTags("Bancoss")
            .Produces<NexusResult<BancosOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] IBancosService bancosService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<BancosOutputModel>();

        try
        {
            var bancos = await bancosService.GetByIdAsync(id, cancellationToken);

            if (bancos == null)
            {
                result.AddFailureMessage($"Bancos com ID {id} não encontrado");
                return Results.NotFound(result);
            }

            result.ResultData = bancos;
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
