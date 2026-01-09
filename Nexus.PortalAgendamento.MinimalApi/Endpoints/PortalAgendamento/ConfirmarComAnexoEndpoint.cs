using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class ConfirmarComAnexoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/confirmar-com-anexo", HandleAsync)
            .WithName("ConfirmarComAnexo")
            .WithSummary("Confirma o agendamento utilizando o anexo enviado previamente para vinculo em tratativas.")
            .WithTags("Confirmação via Anexo")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfirmacaoComAnexoInputModel input,
        [FromServices] IPortalAgendamentoService service,
        [FromServices] ILogger<ConfirmarComAnexoEndpoint> logger,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            logger.LogInformation(">>> INÍCIO CONFIRMAÇÃO COM ANEXO. Guid: {Guid}. Data Solicitada: {Dt}",
                input.IdentificadorCliente, input.DataSolicitada);

            // ---------------------------------------------------------
            // 1. VALIDAÇÃO DE SEGURANÇA (Igual ao ConfirmacaoEndpoint)
            // ---------------------------------------------------------
            var validacaoInput = new ValidadeTokenInputModel { IdentificadorCliente = input.IdentificadorCliente };
            var validacaoResult = await service.ValidarTokenAsync(validacaoInput, ct);

            if (!validacaoResult.IsSuccess || validacaoResult.ResultData == null)
            {
                output.Token = new ValidadeTokenOutputModel { TokenValido = false };
                output.Mensagem = "Token inválido ou não encontrado.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            output.Token = validacaoResult.ResultData;

            // Trava: Expiração do Link
            if (!output.Token.TokenValido)
            {
                output.Mensagem = "O link de agendamento expirou.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            // Trava: Data Passada
            if (input.DataSolicitada.Date < DateTime.Now.Date)
            {
                output.Mensagem = "A data solicitada não pode ser no passado.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage(); // Retorna 200 com mensagem de erro amigável
                return Results.Ok(result);
            }

            // ---------------------------------------------------------
            // 2. PROCESSAMENTO (Recupera Anexo + Agenda + Vincula)
            // ---------------------------------------------------------

            // Convertemos para o InputModel genérico que o método de confirmação usa internamente se necessário,
            // ou passamos os dados diretamente para o método novo.
            var inputConfirmacao = new ConfirmacaoInputModel
            {
                IdentificadorCliente = input.IdentificadorCliente
            };

            logger.LogInformation("[DEBUG] Chamando AgendarComAnexoTempAsync...");

            var responseService = await service.AgendarComAnexoTempAsync(
                inputConfirmacao,
                input.DataSolicitada,
                ct
            );

            if (!responseService.IsSuccess)
            {
                // Se falhou (ex: Arquivo expirou, erro técnico), retornamos 400
                return Results.BadRequest(responseService);
            }

            // ---------------------------------------------------------
            // 3. RETORNO
            // ---------------------------------------------------------
            // O Service já monta o OutputModel completo com mensagens e status
            return Results.Ok(responseService);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro crítico no ConfirmarComAnexoEndpoint");
            result.AddFailureMessage($"Erro interno: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}