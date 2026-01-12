using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class UploadAnexoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/upload-analise", HandleAsync)
            .WithName("UploadAnexoAnalise")
            .WithSummary("Realiza o upload do anexo e extrai sugestões de data.")
            .WithDescription("Recebe um arquivo PDF (multipart/form-data), armazena temporariamente e utiliza OCR para sugerir datas encontradas no documento. O identificador do cliente pode ser enviado no Form-Data ou na Query String.")
            .WithTags("Confirmação via Anexo")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<NexusResult<AnaliseAnexoOutputModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        HttpRequest request,
        [FromServices] IPortalAgendamentoService service,
        [FromServices] ILogger<UploadAnexoEndpoint> logger,
        CancellationToken ct)
    {
        var result = new NexusResult<AnaliseAnexoOutputModel>();

        try
        {
            if (!request.HasFormContentType)
            {
                logger.LogWarning("[UploadAnexo] Tentativa de upload com Content-Type inválido: {Type}", request.ContentType);
                return Results.BadRequest(new { Message = "Formato inválido. Esperado 'multipart/form-data'." });
            }

            var form = await request.ReadFormAsync(ct);
            var file = form.Files.GetFile("arquivo");

            string? guidStr = form["identificadorCliente"];

            if (string.IsNullOrWhiteSpace(guidStr))
            {
                guidStr = request.Query["identificadorCliente"];
            }

            if (file is null || file.Length == 0)
            {
                logger.LogWarning("[UploadAnexo] Arquivo não fornecido ou vazio.");
                result.AddFailureMessage("É obrigatório enviar o arquivo (campo 'arquivo').");
                return Results.BadRequest(result);
            }

            if (string.IsNullOrWhiteSpace(guidStr) || !Guid.TryParse(guidStr, out var guid))
            {
                logger.LogWarning("[UploadAnexo] Identificador do cliente inválido ou ausente: '{GuidStr}'", guidStr);
                result.AddFailureMessage("Identificador do Cliente inválido ou não informado.");
                return Results.BadRequest(result);
            }

            logger.LogInformation("[UploadAnexo] Recebendo arquivo '{Nome}' ({Tamanho} bytes) para Cliente {ClienteId}",
                file.FileName, file.Length, guid);

            var serviceResult = await service.UploadAnaliseAnexoAsync(guid, file, ct);

            if (!serviceResult.IsSuccess)
            {
                logger.LogWarning("[UploadAnexo] Falha no processamento: {Erro}", serviceResult.Messages.FirstOrDefault()?.Description);
                return Results.BadRequest(serviceResult);
            }

            var qtdDatas = serviceResult.ResultData?.DatasLocalizadas.Count ?? 0;
            logger.LogInformation("[UploadAnexo] Sucesso. Datas extraídas: {Qtd}", qtdDatas);

            return Results.Ok(serviceResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[UploadAnexo] Erro crítico durante o upload.");
            result.AddFailureMessage($"Erro interno ao processar upload: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}