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
            .WithName("Aceitar Sugestão de Data")
            .WithSummary("Aceita sugestão de data e realiza o agendamento utilizando a data sugerida e horário padrão 08:00:00")
            .WithTags("Aceitar Sugestão")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfirmacaoInputModel input,
        [FromServices] IPortalAgendamentoService service,
        [FromServices] ILogger<ConfirmacaoEndpoint> logger, // <--- INJETADO LOGGER
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            logger.LogInformation("=== [DEBUG] INÍCIO CONFIRMAÇÃO ===");

            // 1. VALIDAÇÃO DO TOKEN
            var validacaoInput = new ValidadeTokenInputModel { IdentificadorCliente = input.IdentificadorCliente };
            var validacaoResult = await service.ValidarTokenAsync(validacaoInput, ct);

            if (!validacaoResult.IsSuccess || validacaoResult.ResultData == null)
            {
                logger.LogWarning("[DEBUG] Token inválido ou nulo.");
                output.Token = new ValidadeTokenOutputModel { TokenValido = false };
                output.Mensagem = "Token inválido ou não encontrado.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            output.Token = validacaoResult.ResultData;

            // TRAVA 1: Expiração do Link
            if (!output.Token.TokenValido)
            {
                logger.LogWarning("[DEBUG] Token expirado.");
                output.Mensagem = "O link de agendamento expirou.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            // TRAVA 2: Validação da DATA
            if (output.Token.DataSugestaoAgendamento.HasValue)
            {
                output.DataSugestao = output.Token.DataSugestaoAgendamento;

                // Debug da Data
                logger.LogInformation("... {Dt} vs {Hoje}",
                output.DataSugestao?.ToString("dd/MM/yyyy"),
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                if (output.DataSugestao.Value.Date <= DateTime.Now.Date)
                {
                    logger.LogWarning("[DEBUG] Tentativa de agendar para data passada.");
                    output.Mensagem = "A data sugerida para este agendamento já passou. Por favor, solicite uma alteração de data.";
                    output.NotasFiscais = new List<NotaFiscalOutputModel>();
                    result.ResultData = output;
                    result.AddDefaultSuccessMessage();
                    return Results.Ok(result);
                }
            }

            // 2. BUSCAR NOTAS
            var notasModel = await service.GetNotasConhecimento(input.IdentificadorCliente, ct);
            output.NotasFiscais = notasModel?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            logger.LogInformation("[DEBUG] Notas Encontradas: {Qtd}", output.NotasFiscais.Count);

            if (!output.NotasFiscais.Any())
            {
                output.Mensagem = "Nenhuma nota encontrada.";
                result.ResultData = output;
                return Results.Ok(result);
            }

            // 3. PROCESSAR AGENDAMENTO
            if (output.DataSugestao.HasValue)
            {
                logger.LogInformation("[DEBUG] Chamando Service.ConfirmarAgendamento...");

                var processamento = await service.ConfirmarAgendamento(
                    input.IdentificadorCliente,
                    output.DataSugestao.Value,
                    output.NotasFiscais,
                    ct
                );

                if (processamento.ResultData != null)
                {
                    output.ResultadoProcessamento = processamento.ResultData;

                    // --- DEBUG DOS STATUS RETORNADOS ---
                    foreach (var item in output.ResultadoProcessamento)
                    {
                        logger.LogInformation("[DEBUG] Item Retorno -> Nota: {Nr} | Status: '{St}' | Agendado Bool: {Ag}",
                            item.NrNota, item.Status, item.Agendado);
                    }
                    // -----------------------------------
                }

                if (!processamento.IsSuccess)
                {
                    logger.LogError("[DEBUG] Falha no Service: {Erro}", processamento.Messages.FirstOrDefault()?.Description);
                    result.AddFailureMessage("Houve uma falha técnica ao processar.");
                    return Results.BadRequest(result);
                }
            }

            // -----------------------------------------------------------
            // 4. MENSAGEM FINAL (LÓGICA CORRIGIDA E DEBUGADA)
            // -----------------------------------------------------------
            int qtdTotal = output.NotasFiscais.Count;

            // Contamos quem foi agendado AGORA (Insert Realizado = true)
            int qtdNovos = output.ResultadoProcessamento.Count(x => x.Agendado);

            // CORREÇÃO AQUI: O Service retorna "Já Agendado", não "Agendado".
            // Se você procurar por "Agendado", vai dar 0 sempre.
            int qtdJaAgendado = output.ResultadoProcessamento
                .Count(x => string.Equals(x.Status, "Já Agendado", StringComparison.OrdinalIgnoreCase));

            // Debug da Contagem
            logger.LogInformation("[DEBUG] CONTAGEM FINAL: Total={T} | Novos={N} | JáAgendados={J}",
                qtdTotal, qtdNovos, qtdJaAgendado);

            int totalSucesso = qtdNovos + qtdJaAgendado;
            string dt = output.DataSugestao?.ToString("dd/MM/yyyy") ?? "";

            if (totalSucesso == qtdTotal && qtdTotal > 0)
            {
                if (qtdNovos > 0)
                {
                    // Cenário: Pelo menos um item precisou de INSERT/UPDATE no banco
                    output.Mensagem = $"Sucesso! Agendamento confirmado para {dt}.";
                    logger.LogInformation("[DEBUG] Cenário: SUCESSO NOVO");
                }
                else
                {
                    // Cenário: Todos retornaram "Já Agendado"
                    output.Mensagem = $"Agendamento já consta confirmado para {dt}.";
                    logger.LogInformation("[DEBUG] Cenário: JÁ CONSTA CONFIRMADO");
                }
            }
            else if (totalSucesso > 0)
            {
                output.Mensagem = $"Agendamento parcial ({totalSucesso}/{qtdTotal}) realizado. Verifique os itens.";
                logger.LogWarning("[DEBUG] Cenário: PARCIAL");
            }
            else
            {
                output.Mensagem = "Não foi possível confirmar o agendamento. Verifique os motivos na lista abaixo.";
                logger.LogError("[DEBUG] Cenário: FALHA TOTAL");
            }

            result.ResultData = output;
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DEBUG] EXCEPTION NO ENDPOINT");
            result.AddFailureMessage($"Erro: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}