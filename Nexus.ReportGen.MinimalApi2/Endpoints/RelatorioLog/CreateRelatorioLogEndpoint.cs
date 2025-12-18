using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioLog;

/// <summary>
/// Endpoint para operações CRUD de relatoriologs (Create, Update, Delete)
/// </summary>
public class CreateRelatorioLogEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioLog");

        // CREATE - POST /RelatorioLog
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioLog")
            .WithSummary("Cria um novo relatoriolog")
            .WithDescription("Cria um novo relatoriolog no sistema")
            .WithTags("RelatorioLogs")
            .Produces<NexusResult<RelatorioLogInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioLog/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioLog")
            .WithSummary("Atualiza um relatoriolog existente")
            .WithDescription("Atualiza os dados de um relatoriolog existente")
            .WithTags("RelatorioLogs")
            .Produces<NexusResult<RelatorioLogInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioLog/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioLog")
            .WithSummary("Exclui um relatoriolog")
            .WithDescription("Realiza exclusão lógica de um relatoriolog")
            .WithTags("RelatorioLogs")
            .Produces<NexusResult<RelatorioLogInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioLogInputModel request,
        [FromServices] IRelatorioLogService relatoriologService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioLogInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriologService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioLog/{result.ResultData?.CodLog}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioLogInputModel request,
        [FromServices] IRelatorioLogService relatoriologService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioLogInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriolog são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodLog = id;

            result = await relatoriologService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioLogInputModel request,
        [FromServices] IRelatorioLogService relatoriologService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioLogInputModel>();

        try
        {
            result = await relatoriologService.DeleteAsync(request, cancellationToken);

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
