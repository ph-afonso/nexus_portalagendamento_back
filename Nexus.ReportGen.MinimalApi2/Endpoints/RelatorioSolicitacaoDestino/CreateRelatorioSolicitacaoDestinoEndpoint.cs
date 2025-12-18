using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioSolicitacaoDestino;

/// <summary>
/// Endpoint para operações CRUD de relatoriosolicitacaodestinos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioSolicitacaoDestinoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioSolicitacaoDestino");

        // CREATE - POST /RelatorioSolicitacaoDestino
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioSolicitacaoDestino")
            .WithSummary("Cria um novo relatoriosolicitacaodestino")
            .WithDescription("Cria um novo relatoriosolicitacaodestino no sistema")
            .WithTags("RelatorioSolicitacaoDestinos")
            .Produces<NexusResult<RelatorioSolicitacaoDestinoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioSolicitacaoDestino/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioSolicitacaoDestino")
            .WithSummary("Atualiza um relatoriosolicitacaodestino existente")
            .WithDescription("Atualiza os dados de um relatoriosolicitacaodestino existente")
            .WithTags("RelatorioSolicitacaoDestinos")
            .Produces<NexusResult<RelatorioSolicitacaoDestinoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioSolicitacaoDestino/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioSolicitacaoDestino")
            .WithSummary("Exclui um relatoriosolicitacaodestino")
            .WithDescription("Realiza exclusão lógica de um relatoriosolicitacaodestino")
            .WithTags("RelatorioSolicitacaoDestinos")
            .Produces<NexusResult<RelatorioSolicitacaoDestinoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioSolicitacaoDestinoInputModel request,
        [FromServices] IRelatorioSolicitacaoDestinoService relatoriosolicitacaodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoDestinoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriosolicitacaodestinoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioSolicitacaoDestino/{result.ResultData?.CodSolicitacaoDestino}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioSolicitacaoDestinoInputModel request,
        [FromServices] IRelatorioSolicitacaoDestinoService relatoriosolicitacaodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoDestinoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriosolicitacaodestino são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodSolicitacaoDestino = id;

            result = await relatoriosolicitacaodestinoService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioSolicitacaoDestinoInputModel request,
        [FromServices] IRelatorioSolicitacaoDestinoService relatoriosolicitacaodestinoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoDestinoInputModel>();

        try
        {
            result = await relatoriosolicitacaodestinoService.DeleteAsync(request, cancellationToken);

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
