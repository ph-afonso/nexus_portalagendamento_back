using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioSolicitacao;

/// <summary>
/// Endpoint para operações CRUD de relatoriosolicitacaos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioSolicitacaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioSolicitacao");

        // CREATE - POST /RelatorioSolicitacao
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioSolicitacao")
            .WithSummary("Cria um novo relatoriosolicitacao")
            .WithDescription("Cria um novo relatoriosolicitacao no sistema")
            .WithTags("RelatorioSolicitacaos")
            .Produces<NexusResult<RelatorioSolicitacaoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioSolicitacao/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioSolicitacao")
            .WithSummary("Atualiza um relatoriosolicitacao existente")
            .WithDescription("Atualiza os dados de um relatoriosolicitacao existente")
            .WithTags("RelatorioSolicitacaos")
            .Produces<NexusResult<RelatorioSolicitacaoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioSolicitacao/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioSolicitacao")
            .WithSummary("Exclui um relatoriosolicitacao")
            .WithDescription("Realiza exclusão lógica de um relatoriosolicitacao")
            .WithTags("RelatorioSolicitacaos")
            .Produces<NexusResult<RelatorioSolicitacaoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioSolicitacaoInputModel request,
        [FromServices] IRelatorioSolicitacaoService relatoriosolicitacaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriosolicitacaoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioSolicitacao/{result.ResultData?.CodRelatorioSolicitacao}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioSolicitacaoInputModel request,
        [FromServices] IRelatorioSolicitacaoService relatoriosolicitacaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriosolicitacao são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodRelatorioSolicitacao = id;

            result = await relatoriosolicitacaoService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioSolicitacaoInputModel request,
        [FromServices] IRelatorioSolicitacaoService relatoriosolicitacaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioSolicitacaoInputModel>();

        try
        {
            result = await relatoriosolicitacaoService.DeleteAsync(request, cancellationToken);

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
