// Endpoints/PortalAgendamento/CreateVoucherTratativaEndpoint.cs
using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;

namespace Nexus.Sample.MinimalApi.Endpoints.PortalAgendamento;

public class CreateVoucherTratativaEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/voucher/{identificadorCliente:guid}", HandleAsync)
            .DisableAntiforgery()
            .WithName("CreateVoucherTratativa")
            .WithSummary("Criar tratativa de ocorrência (VOUCHER) e anexar arquivo")
            .WithDescription("Insere em TB_OCORRENCIAS_TRATATIVAS, TB_OCORRENCIAS_TRATATIVAS_ARQUIVOS e salva o arquivo no servidor. Em caso de erro, envia e-mail com o VOUCHER em anexo.")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid identificadorCliente,
        [FromForm] IFormFile request,
        [FromServices] IPortalAgendamentoService portalAgendamentoService,
        CancellationToken cancellationToken)
    {
        var result = new NexusResult<bool>();

        try
        {
            if (identificadorCliente == Guid.Empty || request is null)
            {
                result.AddFailureMessage("IdentificadorCliente e arquivo são obrigatórios.");
                return Results.BadRequest(result);
            }

            var op = await portalAgendamentoService.CreateVoucherTratativaAsync(identificadorCliente, request, cancellationToken);

            if (!op.IsSuccess)
            {
                return Results.BadRequest(op);
            }

            return Results.Ok(op);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage(ex.Message);
            return Results.BadRequest(result);
        }
    }
}
