using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class SolicitarAlteracaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("portal-agendamento/solicitar-alteracao")
            .WithTags("Solicitar Outra Data");

        // GET: Carrega dados para a tela (Token + Notas)
        group.MapGet("/{identificadorCliente}", HandleGetAsync)
            .WithName("ConsultarAlteracao")
            .WithSummary("Obter Dados Solicitação")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // POST: Executa a alteração (Grava nova data)
        group.MapPost("/", HandlePostAsync)
            .WithName("ProcessarAlteracao")
            .WithSummary("Agendar Com Outra Data")
            .Produces<NexusResult<ConfirmacaoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    // --- HANDLE GET ---
    private static async Task<IResult> HandleGetAsync(
        [FromRoute] Guid identificadorCliente,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            // 1. Validar Token
            var validacao = await service.ValidarTokenAsync(new ValidadeTokenInputModel { IdentificadorCliente = identificadorCliente }, ct);

            if (!validacao.IsSuccess || validacao.ResultData == null || !validacao.ResultData.TokenValido)
            {
                output.Mensagem = "Token inválido ou expirado.";
                result.ResultData = output;
                result.AddDefaultSuccessMessage(); // Retorna 200 com msg de erro no corpo para o front tratar
                return Results.Ok(result);
            }

            output.Token = validacao.ResultData;
            output.DataSugestao = output.Token.DataSugestaoAgendamento;

            // 2. Buscar Notas
            var notasData = await service.GetNotasConhecimento(identificadorCliente, ct);
            output.NotasFiscais = notasData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            if (!output.NotasFiscais.Any())
                output.Mensagem = "Nenhuma nota encontrada para alteração.";

            result.ResultData = output;
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage($"Erro interno: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    // --- HANDLE POST ---
    private static async Task<IResult> HandlePostAsync(
        [FromBody] SolicitarAlteracaoInputModel input,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        var result = new NexusResult<ConfirmacaoOutputModel>();
        var output = new ConfirmacaoOutputModel();

        try
        {
            // 1. Validar Token novamente (Segurança)
            var validacao = await service.ValidarTokenAsync(new ValidadeTokenInputModel { IdentificadorCliente = input.IdentificadorCliente }, ct);

            if (!validacao.IsSuccess || validacao.ResultData == null || !validacao.ResultData.TokenValido)
            {
                output.Mensagem = "Token inválido ou expirado.";
                result.ResultData = output;
                return Results.Ok(result);
            }
            output.Token = validacao.ResultData;

            // 2. Buscar Notas (para processar sobre elas)
            var notasData = await service.GetNotasConhecimento(input.IdentificadorCliente, ct);
            output.NotasFiscais = notasData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();

            if (!output.NotasFiscais.Any())
            {
                output.Mensagem = "Nenhuma nota disponível para agendamento.";
                result.ResultData = output;
                return Results.Ok(result);
            }

            // 3. Montar Data Final (Data + Hora Solicitada)
            // A lógica de negócio no Service usa DateTime para gravar.
            // Se a hora vier "08:00", combinamos com a DataSolicitada.
            DateTime dataFinal = input.DataSolicitada.Date;
            if (TimeSpan.TryParse(input.HoraSolicitada, out var time))
            {
                dataFinal = dataFinal.Add(time);
            }
            else
            {
                dataFinal = dataFinal.AddHours(8); // Fallback padrão
            }

            // 4. Processar Agendamento (Reutilizando a lógica robusta do Service)
            // Nota: O service atual usa um Periodo fixo (4) no repository. 
            // Se precisar passar o input.Periodo, teríamos que alterar a assinatura do Service.
            // Por enquanto, enviamos a Data/Hora escolhida pelo usuário.
            var processamento = await service.ConfirmarAgendamento(
                input.IdentificadorCliente,
                dataFinal,
                output.NotasFiscais,
                ct);

            if (!processamento.IsSuccess)
            {
                result.AddFailureMessage("Falha técnica ao processar a alteração.");
                return Results.BadRequest(result);
            }

            output.ResultadoProcessamento = processamento.ResultData ?? new List<AgendamentoDetalheModel>();

            // 5. Mensagem Final
            int qtdSucesso = output.ResultadoProcessamento.Count(x => x.Agendado);
            output.Mensagem = qtdSucesso > 0
                ? $"Solicitação de alteração realizada para {dataFinal:dd/MM/yyyy HH:mm}."
                : "Não foi possível realizar a alteração. Verifique os status.";

            result.ResultData = output;
            result.AddDefaultSuccessMessage();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            result.AddFailureMessage($"Erro ao solicitar alteração: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}