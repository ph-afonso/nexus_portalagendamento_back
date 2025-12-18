using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;

namespace Nexus.Sample.MinimalApi.Endpoints.Bancos;

/// <summary>
/// Endpoint para operações CRUD de bancoss (Create, Update, Delete)
/// </summary>
public class CreateBancosEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/Bancos");

        // CREATE - POST /Bancos
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateBancos")
            .WithSummary("Cria um novo bancos")
            .WithDescription("Cria um novo bancos no sistema")
            .WithTags("Bancoss")
            .Produces<NexusResult<BancosInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /Bancos/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateBancos")
            .WithSummary("Atualiza um bancos existente")
            .WithDescription("Atualiza os dados de um bancos existente")
            .WithTags("Bancoss")
            .Produces<NexusResult<BancosInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /Bancos/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteBancos")
            .WithSummary("Exclui um bancos")
            .WithDescription("Realiza exclusão lógica de um bancos")
            .WithTags("Bancoss")
            .Produces<NexusResult<BancosInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] BancosInputModel request,
        [FromServices] IBancosService bancosService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<BancosInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await bancosService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/Bancos/{result.ResultData?.CodBancos}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] BancosInputModel request,
        [FromServices] IBancosService bancosService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<BancosInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do bancos são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodBancos = id;

            result = await bancosService.UpdateAsync(request, cancellationToken);

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
        [FromBody] BancosInputModel request,
        [FromServices] IBancosService bancosService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<BancosInputModel>();

        try
        {
            result = await bancosService.DeleteAsync(request, cancellationToken);

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
