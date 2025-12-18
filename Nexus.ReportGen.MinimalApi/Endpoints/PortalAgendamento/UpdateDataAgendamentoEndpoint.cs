using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Services;

namespace Nexus.Sample.MinimalApi.Endpoints.PortalAgendamento;

/// <summary>
/// Endpoint para buscar atualizar a data do agendamento
/// </summary>
public class UpdateDataAgendamentoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/salvar/{identificadorCliente:guid}", HandleAsync)
            .WithName("UpdateDataAgendamento")
            .WithSummary("Atualizar Data de Agendamento")
            .WithDescription("Atualizar Data de Agendamento")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<PortalAgendamentoInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid identificadorCliente,
        [FromBody] PortalAgendamentoInputModel request,
        [FromServices] IPortalAgendamentoService portalAgendamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<PortalAgendamentoInputModel>();

        try
        {
            if (request == null)
            {
                result.AddFailureMessage("Dados do request são obrigatórios");
                return Results.BadRequest(result);
            }

            if (request.IdentificadorClientes == Guid.Empty || request.IdentificadorClientes is null)
            {
                request.IdentificadorClientes = identificadorCliente;
            }
            else if (request.IdentificadorClientes != identificadorCliente)
            {
                result.AddFailureMessage("Identificador do cliente não corresponde ao identificador na rota");
                return Results.BadRequest(result);
            }

            result = await portalAgendamentoService.UpdateDataAgendamento(request, cancellationToken);

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
}
