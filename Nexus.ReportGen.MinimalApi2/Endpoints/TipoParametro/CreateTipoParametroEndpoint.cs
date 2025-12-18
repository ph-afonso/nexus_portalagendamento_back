using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.TipoParametro;

/// <summary>
/// Endpoint para operações CRUD de tipoparametros (Create, Update, Delete)
/// </summary>
public class CreateTipoParametroEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/TipoParametro");

        // CREATE - POST /TipoParametro
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateTipoParametro")
            .WithSummary("Cria um novo tipoparametro")
            .WithDescription("Cria um novo tipoparametro no sistema")
            .WithTags("TipoParametros")
            .Produces<NexusResult<TipoParametroInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /TipoParametro/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateTipoParametro")
            .WithSummary("Atualiza um tipoparametro existente")
            .WithDescription("Atualiza os dados de um tipoparametro existente")
            .WithTags("TipoParametros")
            .Produces<NexusResult<TipoParametroInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /TipoParametro/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteTipoParametro")
            .WithSummary("Exclui um tipoparametro")
            .WithDescription("Realiza exclusão lógica de um tipoparametro")
            .WithTags("TipoParametros")
            .Produces<NexusResult<TipoParametroInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] TipoParametroInputModel request,
        [FromServices] ITipoParametroService tipoparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<TipoParametroInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await tipoparametroService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/TipoParametro/{result.ResultData?.CodTipoParametro}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] TipoParametroInputModel request,
        [FromServices] ITipoParametroService tipoparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<TipoParametroInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do tipoparametro são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodTipoParametro = id;

            result = await tipoparametroService.UpdateAsync(request, cancellationToken);

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
        [FromBody] TipoParametroInputModel request,
        [FromServices] ITipoParametroService tipoparametroService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<TipoParametroInputModel>();

        try
        {
            result = await tipoparametroService.DeleteAsync(request, cancellationToken);

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
