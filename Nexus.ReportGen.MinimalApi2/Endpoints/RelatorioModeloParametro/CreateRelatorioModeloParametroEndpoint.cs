using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioModeloParametro;

/// <summary>
/// Endpoint para operações CRUD de relatoriomodeloparametros (Create, Update, Delete)
/// </summary>
public class CreateRelatorioModeloParametroEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioModeloParametro");

        // CREATE - POST /RelatorioModeloParametro
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioModeloParametro")
            .WithSummary("Cria um novo relatoriomodeloparametro")
            .WithDescription("Cria um novo relatoriomodeloparametro no sistema")
            .WithTags("RelatorioModeloParametros")
            .Produces<NexusResult<RelatorioModeloParametroInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioModeloParametro/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioModeloParametro")
            .WithSummary("Atualiza um relatoriomodeloparametro existente")
            .WithDescription("Atualiza os dados de um relatoriomodeloparametro existente")
            .WithTags("RelatorioModeloParametros")
            .Produces<NexusResult<RelatorioModeloParametroInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioModeloParametro/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioModeloParametro")
            .WithSummary("Exclui um relatoriomodeloparametro")
            .WithDescription("Realiza exclusão lógica de um relatoriomodeloparametro")
            .WithTags("RelatorioModeloParametros")
            .Produces<NexusResult<RelatorioModeloParametroInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioModeloParametroInputModel request,
        [FromServices] IRelatorioModeloParametroService relatoriomodeloparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloParametroInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriomodeloparametroService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioModeloParametro/{result.ResultData?.CodParametro}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioModeloParametroInputModel request,
        [FromServices] IRelatorioModeloParametroService relatoriomodeloparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloParametroInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriomodeloparametro são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodParametro = id;

            result = await relatoriomodeloparametroService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioModeloParametroInputModel request,
        [FromServices] IRelatorioModeloParametroService relatoriomodeloparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioModeloParametroInputModel>();

        try
        {
            result = await relatoriomodeloparametroService.DeleteAsync(request, cancellationToken);

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
