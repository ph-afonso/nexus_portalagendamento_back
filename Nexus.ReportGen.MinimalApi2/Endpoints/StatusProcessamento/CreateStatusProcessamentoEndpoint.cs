using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.StatusProcessamento;

/// <summary>
/// Endpoint para operações CRUD de statusprocessamentos (Create, Update, Delete)
/// </summary>
public class CreateStatusProcessamentoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/StatusProcessamento");

        // CREATE - POST /StatusProcessamento
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateStatusProcessamento")
            .WithSummary("Cria um novo statusprocessamento")
            .WithDescription("Cria um novo statusprocessamento no sistema")
            .WithTags("StatusProcessamentos")
            .Produces<NexusResult<StatusProcessamentoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /StatusProcessamento/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateStatusProcessamento")
            .WithSummary("Atualiza um statusprocessamento existente")
            .WithDescription("Atualiza os dados de um statusprocessamento existente")
            .WithTags("StatusProcessamentos")
            .Produces<NexusResult<StatusProcessamentoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /StatusProcessamento/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteStatusProcessamento")
            .WithSummary("Exclui um statusprocessamento")
            .WithDescription("Realiza exclusão lógica de um statusprocessamento")
            .WithTags("StatusProcessamentos")
            .Produces<NexusResult<StatusProcessamentoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] StatusProcessamentoInputModel request,
        [FromServices] IStatusProcessamentoService statusprocessamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<StatusProcessamentoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await statusprocessamentoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/StatusProcessamento/{result.ResultData?.CodStatusProcessamento}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] StatusProcessamentoInputModel request,
        [FromServices] IStatusProcessamentoService statusprocessamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<StatusProcessamentoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do statusprocessamento são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodStatusProcessamento = id;

            result = await statusprocessamentoService.UpdateAsync(request, cancellationToken);

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
        [FromBody] StatusProcessamentoInputModel request,
        [FromServices] IStatusProcessamentoService statusprocessamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<StatusProcessamentoInputModel>();

        try
        {
            result = await statusprocessamentoService.DeleteAsync(request, cancellationToken);

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
