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
            .WithSummary("Confirmação")
            .WithDescription("Aceita o agendamento conforme solicitado.")
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
            // 1. Validar Token e Obtenção de Status
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
                output.Mensagem = "O link de agendamento expirou (48h excedidas).";
                result.ResultData = output;
                result.AddDefaultSuccessMessage();
                return Results.Ok(result);
            }

            if (output.Token.DataSugestaoAgendamento.HasValue)
            {
                output.DataSugestaoAgendamento = output.Token.DataSugestaoAgendamento;
            }

            // 2. Buscar Notas Fiscais (Ainda precisa ir no banco pois são tabelas filhas)
            var notasResult = await service.GetNotasConhecimento(input.IdentificadorCliente, ct);
            if (notasResult != null && notasResult.NotasFiscais != null)
            {
                output.NotasFiscais = notasResult.NotasFiscais;
            }

            // 3. Mensagem
            var dataFormatada = output.DataSugestaoAgendamento.HasValue
                ? output.DataSugestaoAgendamento.Value.ToString("dd/MM/yyyy")
                : "DATA_INDEFINIDA";

            output.Mensagem = $"Agendamento confirmado para dia {dataFormatada} às 08:00:00.";

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