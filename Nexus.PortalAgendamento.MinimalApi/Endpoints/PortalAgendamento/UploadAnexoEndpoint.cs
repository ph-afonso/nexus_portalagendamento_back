using Microsoft.AspNetCore.Mvc;
using Nexus.Framework.Common;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel; // <--- ESSE USING ESTAVA FALTANDO
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;
using Nexus.PortalAgendamento.MinimalApi.Common;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

public class UploadAnexoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("portal-agendamento/upload-analise", HandleAsync)
            .WithName("UploadAnexoAnalise")
            .WithSummary("Faz upload do anexo e retorna datas e horários encontrados.")
            .WithTags("Confirmação via Anexo")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<NexusResult<AnaliseAnexoOutputModel>>();

    private static async Task<IResult> HandleAsync(
        HttpRequest request,
        [FromServices] IPortalAgendamentoService service,
        CancellationToken ct)
    {
        // Validação básica do Form-Data
        if (!request.HasFormContentType)
            return Results.BadRequest("Formato inválido. Esperado multipart/form-data.");

        var form = await request.ReadFormAsync(ct);
        var file = form.Files.GetFile("arquivo");

        // Tenta pegar o GUID do form, senão tenta da Query string
        var guidStr = form["identificadorCliente"].ToString();
        if (string.IsNullOrEmpty(guidStr))
        {
            guidStr = request.Query["identificadorCliente"].ToString();
        }

        if (file == null || string.IsNullOrEmpty(guidStr) || !Guid.TryParse(guidStr, out var guid))
        {
            var erro = new NexusResult<object>();
            erro.AddFailureMessage("Arquivo e IdentificadorCliente são obrigatórios.");
            return Results.BadRequest(erro);
        }

        // Chama o serviço
        var result = await service.UploadAnaliseAnexoAsync(guid, file, ct);

        if (!result.IsSuccess)
            return Results.BadRequest(result);

        return Results.Ok(result);
    }
}