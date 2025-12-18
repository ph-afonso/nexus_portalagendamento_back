using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloDestino;

/// <summary>
/// Endpoint para operações CRUD de relatoriomodelodestinos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioModeloDestinoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioModeloDestino");

        // CREATE - POST /RelatorioModeloDestino
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioModeloDestino")
            .WithSummary("Cria um novo relatoriomodelodestino")
            .WithDescription("Cria um novo relatoriomodelodestino no sistema")
            .WithTags("RelatorioModeloDestinos")
            .Produces<NexusResult<RelatorioModeloDestinoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioModeloDestino/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioModeloDestino")
            .WithSummary("Atualiza um relatoriomodelodestino existente")
            .WithDescription("Atualiza os dados de um relatoriomodelodestino existente")
            .WithTags("RelatorioModeloDestinos")
            .Produces<NexusResult<RelatorioModeloDestinoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioModeloDestino/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioModeloDestino")
            .WithSummary("Exclui um relatoriomodelodestino")
            .WithDescription("Realiza exclusão lógica de um relatoriomodelodestino")
            .WithTags("RelatorioModeloDestinos")
            .Produces<NexusResult<RelatorioModeloDestinoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioModeloDestinoInputModel request,
        [FromServices] IRelatorioModeloDestinoService relatoriomodelodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloDestinoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriomodelodestinoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioModeloDestino/{result.ResultData?.CodModeloDestino}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioModeloDestinoInputModel request,
        [FromServices] IRelatorioModeloDestinoService relatoriomodelodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloDestinoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriomodelodestino são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodModeloDestino = id;

            result = await relatoriomodelodestinoService.UpdateAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Messages?.Any(m => m.Description?.Contains("não encontrado") == true) == true)
                {
                    return Results.NotFound(result);
                }
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleDeleteAsync(
        [FromRoute] int id,
        [FromBody] RelatorioModeloDestinoInputModel request,
        [FromServices] IRelatorioModeloDestinoService relatoriomodelodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloDestinoInputModel>();

        try
        {
            result = await relatoriomodelodestinoService.DeleteAsync(request, cancellationToken);

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
