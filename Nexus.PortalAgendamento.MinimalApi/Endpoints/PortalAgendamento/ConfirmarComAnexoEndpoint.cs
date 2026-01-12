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
            .WithSummary("Confirma o agendamento utilizando um anexo.")
            .WithDescription("Valida o token, verifica a data solicitada e processa o agendamento vinculando o arquivo PDF previamente enviado (Upload) às ocorrências geradas.")
            .WithTags("Agendamento")
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
            logger.LogInformation("[ConfirmacaoAnexo] Iniciando fluxo. Cliente: {ClienteId} | Data Solicitada: {Data}",
                input.IdentificadorCliente, input.DataSolicitada);

            var validacaoResult = await service.ValidarTokenAsync(
                new ValidadeTokenInputModel { IdentificadorCliente = input.IdentificadorCliente }, ct);

            if (!validacaoResult.IsSuccess || validacaoResult.ResultData is null || !validacaoResult.ResultData.TokenValido)
            {
                logger.LogWarning("[ConfirmacaoAnexo] Token inválido ou expirado. Cliente: {ClienteId}", input.IdentificadorCliente);

                output.Token = new ValidadeTokenOutputModel { TokenValido = false };
                output.Mensagem = "O link de agendamento expirou ou é inválido.";

                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            output.Token = validacaoResult.ResultData;

            if (input.DataSolicitada.Date < DateTime.Now.Date)
            {
                logger.LogWarning("[ConfirmacaoAnexo] Tentativa de agendamento retroativo: {Data}", input.DataSolicitada);

                output.Mensagem = "A data solicitada não pode ser no passado.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            var inputConfirmacao = new ConfirmacaoInputModel { IdentificadorCliente = input.IdentificadorCliente };

            var responseService = await service.AgendarComAnexoTempAsync(
                inputConfirmacao,
                input.DataSolicitada,
                ct
            );

            if (!responseService.IsSuccess)
            {
                logger.LogError("[ConfirmacaoAnexo] Falha no processamento: {Erro}", responseService.Messages.FirstOrDefault()?.Description);
                return Results.BadRequest(responseService);
            }

            logger.LogInformation("[ConfirmacaoAnexo] Processo concluído com sucesso para Cliente {ClienteId}.", input.IdentificadorCliente);
            return Results.Ok(responseService);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ConfirmacaoAnexo] Erro crítico não tratado. Cliente: {ClienteId}", input.IdentificadorCliente);
            result.AddFailureMessage($"Erro interno: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}