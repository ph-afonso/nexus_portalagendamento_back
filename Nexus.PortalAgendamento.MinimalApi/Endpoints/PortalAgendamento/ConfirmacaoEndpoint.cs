using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class ConfirmacaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/confirmacao", HandleAsync)
            .WithName("Aceitar Sugestão de Data")
            .WithSummary("Realiza a confirmação do agendamento baseada no token e data sugerida.")
            .WithTags("Agendamento")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfirmacaoInputModel input,
        [FromServices] IPortalAgendamentoService service,
        [FromServices] ILogger<ConfirmacaoEndpoint> logger,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            logger.LogInformation("[Confirmacao] Iniciando processamento para Cliente {ClienteId}", input.IdentificadorCliente);

            var validacaoResult = await service.ValidarTokenAsync(
                new ValidadeTokenInputModel { IdentificadorCliente = input.IdentificadorCliente }, ct);

            if (!validacaoResult.IsSuccess || validacaoResult.ResultData is null || !validacaoResult.ResultData.TokenValido)
            {
                logger.LogWarning("[Confirmacao] Token inválido ou expirado para Cliente {ClienteId}", input.IdentificadorCliente);

                output.Token = new ValidadeTokenOutputModel { TokenValido = false };
                output.Mensagem = "O link de agendamento expirou ou é inválido.";

                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            output.Token = validacaoResult.ResultData;

            if (output.Token.DataSugestaoAgendamento.HasValue)
            {
                output.DataSugestao = output.Token.DataSugestaoAgendamento.Value;

                if (output.DataSugestao.Value.Date <= DateTime.Now.Date)
                {
                    logger.LogWarning("[Confirmacao] Data sugerida ({Data}) já passou. Cliente {ClienteId}",
                        output.DataSugestao.Value.ToShortDateString(), input.IdentificadorCliente);

                    output.Mensagem = "A data sugerida para este agendamento já passou. Por favor, solicite uma alteração de data.";
                    output.NotasFiscais = new List<NotaFiscalOutputModel>();

                    result.ResultData = output;
                    result.AddDefaultSuccessMessage();
                    return Results.Ok(result);
                }
            }

            var notasModel = await service.GetNotasConhecimento(input.IdentificadorCliente, ct);
            output.NotasFiscais = notasModel?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            if (!output.NotasFiscais.Any())
            {
                logger.LogWarning("[Confirmacao] Nenhuma nota fiscal encontrada para Cliente {ClienteId}", input.IdentificadorCliente);
                output.Mensagem = "Nenhuma nota encontrada para processamento.";
                result.ResultData = output;
                return Results.Ok(result);
            }

            if (output.DataSugestao.HasValue)
            {
                var processamento = await service.ConfirmarAgendamento(
                    input.IdentificadorCliente,
                    output.DataSugestao.Value,
                    output.NotasFiscais,
                    ct
                );

                if (!processamento.IsSuccess)
                {
                    logger.LogError("[Confirmacao] Falha no serviço de agendamento: {Erro}", processamento.Messages.FirstOrDefault()?.Description);
                    result.AddFailureMessage("Houve uma falha técnica ao processar o agendamento.");
                    return Results.BadRequest(result);
                }

                output.ResultadoProcessamento = processamento.ResultData ?? new List<AgendamentoDetalheModel>();
            }

            ProcessarMensagemFinal(output, logger);

            result.ResultData = output;
            result.AddDefaultSuccessMessage();

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Confirmacao] Erro não tratado para Cliente {ClienteId}", input.IdentificadorCliente);
            result.AddFailureMessage($"Erro interno: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    private static void ProcessarMensagemFinal(ConfirmacaoOutputModel output, ILogger logger)
    {
        var resultados = output.ResultadoProcessamento ?? new List<AgendamentoDetalheModel>();
        var notas = output.NotasFiscais ?? new List<NotaFiscalOutputModel>();
        int qtdTotal = notas.Count;
        int qtdNovos = resultados.Count(x => x.Agendado);

        int qtdJaAgendado = resultados
            .Count(x => string.Equals(x.Status, "Já Agendado", StringComparison.OrdinalIgnoreCase));

        int totalSucesso = qtdNovos + qtdJaAgendado;
        string dataFormatada = output.DataSugestao?.ToString("dd/MM/yyyy") ?? "-";

        logger.LogInformation("[Confirmacao] Resultado: Total={Total} | Novos={Novos} | Previa={Previa} | Data={Data}",
            qtdTotal, qtdNovos, qtdJaAgendado, dataFormatada);

        if (totalSucesso == qtdTotal && qtdTotal > 0)
        {
            output.Mensagem = qtdNovos > 0
                ? $"Sucesso! Agendamento confirmado para {dataFormatada}."
                : $"Agendamento já consta confirmado para {dataFormatada}.";
        }
        else if (totalSucesso > 0)
        {
            output.Mensagem = $"Agendamento parcial ({totalSucesso}/{qtdTotal}) realizado. Verifique os itens.";
            logger.LogWarning("[Confirmacao] Agendamento parcial detectado.");
        }
        else
        {
            output.Mensagem = "Não foi possível confirmar o agendamento. Verifique os motivos na lista.";
            if (qtdTotal > 0)
            {
                logger.LogError("[Confirmacao] Falha total no agendamento das notas.");
            }
        }
    }
}