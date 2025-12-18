using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloSaida;

/// <summary>
/// Endpoint para operações CRUD de relatoriomodelosaidas (Create, Update, Delete)
/// </summary>
public class CreateRelatorioModeloSaidaEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioModeloSaida");

        // CREATE - POST /RelatorioModeloSaida
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioModeloSaida")
            .WithSummary("Cria um novo relatoriomodelosaida")
            .WithDescription("Cria um novo relatoriomodelosaida no sistema")
            .WithTags("RelatorioModeloSaidas")
            .Produces<NexusResult<RelatorioModeloSaidaInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioModeloSaida/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioModeloSaida")
            .WithSummary("Atualiza um relatoriomodelosaida existente")
            .WithDescription("Atualiza os dados de um relatoriomodelosaida existente")
            .WithTags("RelatorioModeloSaidas")
            .Produces<NexusResult<RelatorioModeloSaidaInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioModeloSaida/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioModeloSaida")
            .WithSummary("Exclui um relatoriomodelosaida")
            .WithDescription("Realiza exclusão lógica de um relatoriomodelosaida")
            .WithTags("RelatorioModeloSaidas")
            .Produces<NexusResult<RelatorioModeloSaidaInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioModeloSaidaInputModel request,
        [FromServices] IRelatorioModeloSaidaService relatoriomodelosaidaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloSaidaInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriomodelosaidaService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioModeloSaida/{result.ResultData?.CodModeloSaida}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioModeloSaidaInputModel request,
        [FromServices] IRelatorioModeloSaidaService relatoriomodelosaidaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloSaidaInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriomodelosaida são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodModeloSaida = id;

            result = await relatoriomodelosaidaService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioModeloSaidaInputModel request,
        [FromServices] IRelatorioModeloSaidaService relatoriomodelosaidaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloSaidaInputModel>();

        try
        {
            result = await relatoriomodelosaidaService.DeleteAsync(request, cancellationToken);

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
