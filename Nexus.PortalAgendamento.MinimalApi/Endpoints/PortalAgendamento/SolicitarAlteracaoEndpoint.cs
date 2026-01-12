using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class SolicitarAlteracaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("portal-agendamento/solicitar-alteracao")
            .WithTags("Solicitação de Alteração");

        group.MapGet("/{identificadorCliente}", HandleGetAsync)
            .WithName("ConsultarDadosAlteracao")
            .WithSummary("Obtém os dados iniciais para a tela de solicitação de nova data.")
            .WithDescription("Valida o token do cliente e retorna a lista de notas fiscais disponíveis para reagendamento.")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/", HandlePostAsync)
            .WithName("ProcessarAlteracaoData")
            .WithSummary("Processa o agendamento para uma data e horário específicos.")
            .WithDescription("Recebe a data/hora manual informada pelo usuário e realiza o agendamento no sistema.")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleGetAsync(
        [FromRoute] Guid identificadorCliente,
        [FromServices] IPortalAgendamentoService service,
        [FromServices] ILogger<SolicitarAlteracaoEndpoint> logger,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            logger.LogInformation("[Alteracao_GET] Consultando dados. Cliente: {ClienteId}", identificadorCliente);

            var validacao = await service.ValidarTokenAsync(
                new ValidadeTokenInputModel { IdentificadorCliente = identificadorCliente }, ct);

            if (!validacao.IsSuccess || validacao.ResultData is null || !validacao.ResultData.TokenValido)
            {
                logger.LogWarning("[Alteracao_GET] Token inválido ou expirado. Cliente: {ClienteId}", identificadorCliente);

                output.Mensagem = "O link de acesso é inválido ou expirou.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            output.Token = validacao.ResultData;
            output.DataSugestao = output.Token.DataSugestaoAgendamento;

            var notasData = await service.GetNotasConhecimento(identificadorCliente, ct);
            output.NotasFiscais = notasData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            if (!output.NotasFiscais.Any())
            {
                logger.LogInformation("[Alteracao_GET] Nenhuma nota encontrada. Cliente: {ClienteId}", identificadorCliente);
                output.Mensagem = "Nenhuma nota encontrada para alteração.";
            }

            result.ResultData = output;
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Alteracao_GET] Erro interno não tratado. Cliente: {ClienteId}", identificadorCliente);
            result.AddFailureMessage($"Erro interno: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    private static async Task<IResult> HandlePostAsync(
        [FromBody] SolicitarAlteracaoInputModel input,
        [FromServices] IPortalAgendamentoService service,
        [FromServices] ILogger<SolicitarAlteracaoEndpoint> logger,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            logger.LogInformation("[Alteracao_POST] Solicitando reagendamento. Cliente: {ClienteId} | Data: {Data} {Hora}",
                input.IdentificadorCliente, input.DataSolicitada.ToShortDateString(), input.HoraSolicitada);

            var validacao = await service.ValidarTokenAsync(
                new ValidadeTokenInputModel { IdentificadorCliente = input.IdentificadorCliente }, ct);

            if (!validacao.IsSuccess || validacao.ResultData is null || !validacao.ResultData.TokenValido)
            {
                logger.LogWarning("[Alteracao_POST] Token inválido. Cliente: {ClienteId}", input.IdentificadorCliente);

                output.Mensagem = "Token inválido ou expirado.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }
            output.Token = validacao.ResultData;

            if (input.DataSolicitada.Date < DateTime.Now.Date)
            {
                logger.LogWarning("[Alteracao_POST] Tentativa de data retroativa: {Data}", input.DataSolicitada);

                output.Mensagem = "A data solicitada não pode ser anterior a hoje.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            var notasData = await service.GetNotasConhecimento(input.IdentificadorCliente, ct);
            output.NotasFiscais = notasData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            if (!output.NotasFiscais.Any())
            {
                result.AddFailureMessage("Nenhuma nota disponível para agendamento.");
                return Results.Ok(result);
            }

            DateTime dataFinal = input.DataSolicitada.Date;

            if (TimeSpan.TryParse(input.HoraSolicitada, out var time))
            {
                dataFinal = dataFinal.Add(time);
            }
            else
            {
                dataFinal = dataFinal.AddHours(8);
            }

            var processamento = await service.ConfirmarAgendamento(
                input.IdentificadorCliente,
                dataFinal,
                output.NotasFiscais,
                ct);

            if (!processamento.IsSuccess)
            {
                logger.LogError("[Alteracao_POST] Falha técnica no serviço. Erro: {Erro}", processamento.Messages.FirstOrDefault()?.Description);
                result.AddFailureMessage("Falha técnica ao processar a solicitação.");
                return Results.BadRequest(result);
            }

            output.ResultadoProcessamento = processamento.ResultData ?? new List<AgendamentoDetalheModel>();

            int qtdSucesso = output.ResultadoProcessamento.Count(x => x.Agendado || x.Status == "Já Agendado");

            output.Mensagem = qtdSucesso > 0
                ? $"Solicitação de alteração realizada com sucesso para {dataFinal:dd/MM/yyyy HH:mm}."
                : "Não foi possível realizar a alteração. Verifique o status das notas.";

            logger.LogInformation("[Alteracao_POST] Finalizado. Sucessos: {Qtd}. Cliente: {ClienteId}", qtdSucesso, input.IdentificadorCliente);

            result.ResultData = output;
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Alteracao_POST] Erro crítico. Cliente: {ClienteId}", input.IdentificadorCliente);
            result.AddFailureMessage($"Erro ao solicitar alteração: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}