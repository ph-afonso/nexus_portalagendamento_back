using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioCategoria;

/// <summary>
/// Endpoint para operações CRUD de relatoriocategorias (Create, Update, Delete)
/// </summary>
public class CreateRelatorioCategoriaEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioCategoria");

        // CREATE - POST /RelatorioCategoria
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioCategoria")
            .WithSummary("Cria um novo relatoriocategoria")
            .WithDescription("Cria um novo relatoriocategoria no sistema")
            .WithTags("RelatorioCategorias")
            .Produces<NexusResult<RelatorioCategoriaInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioCategoria/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioCategoria")
            .WithSummary("Atualiza um relatoriocategoria existente")
            .WithDescription("Atualiza os dados de um relatoriocategoria existente")
            .WithTags("RelatorioCategorias")
            .Produces<NexusResult<RelatorioCategoriaInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioCategoria/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioCategoria")
            .WithSummary("Exclui um relatoriocategoria")
            .WithDescription("Realiza exclusão lógica de um relatoriocategoria")
            .WithTags("RelatorioCategorias")
            .Produces<NexusResult<RelatorioCategoriaInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioCategoriaInputModel request,
        [FromServices] IRelatorioCategoriaService relatoriocategoriaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioCategoriaInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriocategoriaService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioCategoria/{result.ResultData?.CodCategoria}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioCategoriaInputModel request,
        [FromServices] IRelatorioCategoriaService relatoriocategoriaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioCategoriaInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriocategoria são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodCategoria = id;

            result = await relatoriocategoriaService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioCategoriaInputModel request,
        [FromServices] IRelatorioCategoriaService relatoriocategoriaService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioCategoriaInputModel>();

        try
        {
            result = await relatoriocategoriaService.DeleteAsync(request, cancellationToken);

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
