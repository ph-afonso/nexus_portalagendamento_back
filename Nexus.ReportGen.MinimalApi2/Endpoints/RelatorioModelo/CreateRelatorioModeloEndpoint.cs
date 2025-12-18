using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModelo;

/// <summary>
/// Endpoint para operações CRUD de relatoriomodelos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioModeloEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioModelo");

        // CREATE - POST /RelatorioModelo
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioModelo")
            .WithSummary("Cria um novo relatoriomodelo")
            .WithDescription("Cria um novo relatoriomodelo no sistema")
            .WithTags("RelatorioModelos")
            .Produces<NexusResult<RelatorioModeloInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioModelo/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioModelo")
            .WithSummary("Atualiza um relatoriomodelo existente")
            .WithDescription("Atualiza os dados de um relatoriomodelo existente")
            .WithTags("RelatorioModelos")
            .Produces<NexusResult<RelatorioModeloInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioModelo/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioModelo")
            .WithSummary("Exclui um relatoriomodelo")
            .WithDescription("Realiza exclusão lógica de um relatoriomodelo")
            .WithTags("RelatorioModelos")
            .Produces<NexusResult<RelatorioModeloInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioModeloInputModel request,
        [FromServices] IRelatorioModeloService relatoriomodeloService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriomodeloService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioModelo/{result.ResultData?.CodRelatorioModelo}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioModeloInputModel request,
        [FromServices] IRelatorioModeloService relatoriomodeloService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriomodelo são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodRelatorioModelo = id;

            result = await relatoriomodeloService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioModeloInputModel request,
        [FromServices] IRelatorioModeloService relatoriomodeloService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloInputModel>();

        try
        {
            result = await relatoriomodeloService.DeleteAsync(request, cancellationToken);

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
