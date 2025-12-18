using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Services;

namespace Nexus.Sample.MinimalApi.Endpoints.PortalAgendamento;

/// <summary>
/// Endpoint para extrair data de agendamento do PDF
/// </summary>
public class SendEmailAnexoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/email/{identificadorCliente:guid}", HandleAsync)
            .DisableAntiforgery()
            .WithName("SendEmailAnexoEndpoint")
            .WithSummary("Enviar email com anexo")
            .WithDescription("Envia email com anexo")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<EmailPostFixInputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid identificadorCliente,
        [FromForm] IFormFile request,
        [FromServices] IPortalAgendamentoService portalAgendamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<EmailPostFixInputModel>();

        try
        {

            if (request == null)
            {
                result.AddFailureMessage($"Dados do request são obrigatórios");
                return Results.NotFound(result);
            }

            result = await portalAgendamentoService.SendEmailAnexo(identificadorCliente, request, cancellationToken);

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
