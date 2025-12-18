using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioParametroOpcao;

/// <summary>
/// Endpoint para operações CRUD de relatorioparametroopcaos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioParametroOpcaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioParametroOpcao");

        // CREATE - POST /RelatorioParametroOpcao
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioParametroOpcao")
            .WithSummary("Cria um novo relatorioparametroopcao")
            .WithDescription("Cria um novo relatorioparametroopcao no sistema")
            .WithTags("RelatorioParametroOpcaos")
            .Produces<NexusResult<RelatorioParametroOpcaoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioParametroOpcao/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioParametroOpcao")
            .WithSummary("Atualiza um relatorioparametroopcao existente")
            .WithDescription("Atualiza os dados de um relatorioparametroopcao existente")
            .WithTags("RelatorioParametroOpcaos")
            .Produces<NexusResult<RelatorioParametroOpcaoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioParametroOpcao/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioParametroOpcao")
            .WithSummary("Exclui um relatorioparametroopcao")
            .WithDescription("Realiza exclusão lógica de um relatorioparametroopcao")
            .WithTags("RelatorioParametroOpcaos")
            .Produces<NexusResult<RelatorioParametroOpcaoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioParametroOpcaoInputModel request,
        [FromServices] IRelatorioParametroOpcaoService relatorioparametroopcaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioParametroOpcaoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatorioparametroopcaoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioParametroOpcao/{result.ResultData?.CodOpcao}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioParametroOpcaoInputModel request,
        [FromServices] IRelatorioParametroOpcaoService relatorioparametroopcaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioParametroOpcaoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatorioparametroopcao são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodOpcao = id;

            result = await relatorioparametroopcaoService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioParametroOpcaoInputModel request,
        [FromServices] IRelatorioParametroOpcaoService relatorioparametroopcaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioParametroOpcaoInputModel>();

        try
        {
            result = await relatorioparametroopcaoService.DeleteAsync(request, cancellationToken);

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
