using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloPermissao;

/// <summary>
/// Endpoint para operações CRUD de relatoriomodelopermissaos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioModeloPermissaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioModeloPermissao");

        // CREATE - POST /RelatorioModeloPermissao
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioModeloPermissao")
            .WithSummary("Cria um novo relatoriomodelopermissao")
            .WithDescription("Cria um novo relatoriomodelopermissao no sistema")
            .WithTags("RelatorioModeloPermissaos")
            .Produces<NexusResult<RelatorioModeloPermissaoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioModeloPermissao/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioModeloPermissao")
            .WithSummary("Atualiza um relatoriomodelopermissao existente")
            .WithDescription("Atualiza os dados de um relatoriomodelopermissao existente")
            .WithTags("RelatorioModeloPermissaos")
            .Produces<NexusResult<RelatorioModeloPermissaoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioModeloPermissao/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioModeloPermissao")
            .WithSummary("Exclui um relatoriomodelopermissao")
            .WithDescription("Realiza exclusão lógica de um relatoriomodelopermissao")
            .WithTags("RelatorioModeloPermissaos")
            .Produces<NexusResult<RelatorioModeloPermissaoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioModeloPermissaoInputModel request,
        [FromServices] IRelatorioModeloPermissaoService relatoriomodelopermissaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloPermissaoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriomodelopermissaoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioModeloPermissao/{result.ResultData?.CodPermissao}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioModeloPermissaoInputModel request,
        [FromServices] IRelatorioModeloPermissaoService relatoriomodelopermissaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloPermissaoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriomodelopermissao são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodPermissao = id;

            result = await relatoriomodelopermissaoService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioModeloPermissaoInputModel request,
        [FromServices] IRelatorioModeloPermissaoService relatoriomodelopermissaoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloPermissaoInputModel>();

        try
        {
            result = await relatoriomodelopermissaoService.DeleteAsync(request, cancellationToken);

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
