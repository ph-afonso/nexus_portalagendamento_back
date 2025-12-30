using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class ConfirmacaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/confirmacao", HandleAsync)
            .WithName("Confirmação")
            .WithSummary("Confirmação de Agendamento")
            .WithDescription("Valida o token e retorna os dados do agendamento confirmado.")
            .WithTags("PortalAgendamento")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfirmacaoInputModel input,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            // -----------------------------------------------------------
            // 1. VALIDAÇÃO DO TOKEN
            // -----------------------------------------------------------
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

            if (!output.Token.TokenValido)
            {
                output.Mensagem = "O link de agendamento expirou.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            if (output.Token.DataSugestaoAgendamento.HasValue)
            {
                output.DataSugestao = output.Token.DataSugestaoAgendamento;
            }

            // -----------------------------------------------------------
            // 2. BUSCAR NOTAS FISCAIS
            // -----------------------------------------------------------
            PortalAgendamentoOutputModel? notasModel = await service.GetNotasConhecimento(input.IdentificadorCliente, ct);

            // Garante lista instanciada
            output.NotasFiscais = notasModel?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            // -----------------------------------------------------------
            // 3. PROCESSAR AGENDAMENTO
            // -----------------------------------------------------------
            if (output.Token.DataSugestaoAgendamento.HasValue && output.NotasFiscais.Any())
            {
                var dataParaAgendar = output.Token.DataSugestaoAgendamento.Value;

                // O Service agora retorna a lista de detalhes e JÁ ENVIOU O E-MAIL internamente
                var resultService = await service.ConfirmarAgendamento(
                    input.IdentificadorCliente,
                    dataParaAgendar,
                    output.NotasFiscais,
                    ct
                );

                // Mapeia o resultado do processamento para o output
                if (resultService.ResultData != null)
                {
                    output.ResultadoProcessamento = resultService.ResultData;
                }

                // Se houve erro crítico (banco fora, etc), retornamos 400
                if (!resultService.IsSuccess)
                {
                    result.AddFailureMessage("Houve uma falha técnica ao processar os agendamentos.");
                    return Results.BadRequest(result);
                }
            }
            else if (!output.NotasFiscais.Any())
            {
                // Caso especial: Token válido mas sem notas (pode ter sido limpo por outro processo)
                output.Mensagem = "Nenhuma nota fiscal pendente encontrada para este agendamento.";
                result.ResultData = output;
                return Results.Ok(result);
            }

            // -----------------------------------------------------------
            // 4. MENSAGEM FINAL INTELIGENTE
            // -----------------------------------------------------------
            int qtdSucesso = output.ResultadoProcessamento.Count(x => x.Agendado);
            int qtdTotal = output.NotasFiscais.Count;

            string textoData = output.DataSugestao.HasValue
                ? output.DataSugestao.Value.ToString("dd/MM/yyyy")
                : "DATA_INDEFINIDA";

            if (qtdSucesso == qtdTotal && qtdTotal > 0)
            {
                output.Mensagem = $"Sucesso! Todas as {qtdTotal} notas foram agendadas para {textoData}.";
            }
            else if (qtdSucesso > 0)
            {
                output.Mensagem = $"Agendamento Parcial: {qtdSucesso} de {qtdTotal} notas foram agendadas. Verifique os alertas abaixo.";
            }
            else
            {
                output.Mensagem = "Não foi possível agendar as notas. Verifique os motivos na lista abaixo.";
            }

            // Monta o retorno final
            result.ResultData = output;
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage($"Erro ao processar confirmação: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}