using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;
using Nexus.ReportGen.MinimalApi.Common;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

namespace Nexus.ReportGen.MinimalApi.Endpoints.RelatorioDestinoTipo;

/// <summary>
/// Endpoint para operações CRUD de relatoriodestinotipos (Create, Update, Delete)
/// </summary>
public class CreateRelatorioDestinoTipoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/RelatorioDestinoTipo");

        // CREATE - POST /RelatorioDestinoTipo
        group.MapPost("/", HandleCreateAsync)
            .WithName("CreateRelatorioDestinoTipo")
            .WithSummary("Cria um novo relatoriodestinotipo")
            .WithDescription("Cria um novo relatoriodestinotipo no sistema")
            .WithTags("RelatorioDestinoTipos")
            .Produces<NexusResult<RelatorioDestinoTipoInputModel>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // UPDATE - PUT /RelatorioDestinoTipo/{id}
        group.MapPut("/{id:int}", HandleUpdateAsync)
            .WithName("UpdateRelatorioDestinoTipo")
            .WithSummary("Atualiza um relatoriodestinotipo existente")
            .WithDescription("Atualiza os dados de um relatoriodestinotipo existente")
            .WithTags("RelatorioDestinoTipos")
            .Produces<NexusResult<RelatorioDestinoTipoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE - DELETE /RelatorioDestinoTipo/{id}
        group.MapDelete("/{id:int}", HandleDeleteAsync)
            .WithName("DeleteRelatorioDestinoTipo")
            .WithSummary("Exclui um relatoriodestinotipo")
            .WithDescription("Realiza exclusão lógica de um relatoriodestinotipo")
            .WithTags("RelatorioDestinoTipos")
            .Produces<NexusResult<RelatorioDestinoTipoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleCreateAsync(
        [FromBody] RelatorioDestinoTipoInputModel request,
        [FromServices] IRelatorioDestinoTipoService relatoriodestinotipoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioDestinoTipoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados da requisição não foram informados.");
                return Results.BadRequest(result);
            }

            result = await relatoriodestinotipoService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Created($"/RelatorioDestinoTipo/{result.ResultData?.CodDestinoTipo}", result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandleUpdateAsync(
        [FromRoute] int id,
        [FromBody] RelatorioDestinoTipoInputModel request,
        [FromServices] IRelatorioDestinoTipoService relatoriodestinotipoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioDestinoTipoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do relatoriodestinotipo são obrigatórios");
                return Results.BadRequest(result);
            }

            // Garantir que o ID da rota seja usado
            request.CodDestinoTipo = id;

            result = await relatoriodestinotipoService.UpdateAsync(request, cancellationToken);

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
        [FromBody] RelatorioDestinoTipoInputModel request,
        [FromServices] IRelatorioDestinoTipoService relatoriodestinotipoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<RelatorioDestinoTipoInputModel>();

        try
        {
            result = await relatoriodestinotipoService.DeleteAsync(request, cancellationToken);

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
