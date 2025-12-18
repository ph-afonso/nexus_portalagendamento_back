using Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints;

/// <summary>
/// Agrupador de endpoints do Portal Agendamento
/// </summary>
public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        // 1. Health Check Global (Acessível na raiz "http://localhost:xxxx/")
        app.MapGet("/", () => new { status= "online", message = "API Portal Agendamentos V1", timestamp = DateTime.Now })
           .WithTags("Health Check");

        // 2. Grupo Principal da API (Prefixo: /apis.portalagendamento/v1)
        var endpoints = app
            .MapGroup("apis.portalagendamento")
            .MapGroup("v1") // Já emenda o v1 aqui para ficar limpo
            .WithTags("PortalAgendamento");

        // 3. Mapeamento dos Endpoints de Negócio
        CreateVoucherTratativaEndpoint.Map(endpoints);
        GetClienteEndpoint.Map(endpoints);
        GetDataAgendamentoConfirmacaoEndpoint.Map(endpoints);
        GetDataAgendamentoPdfEndpoint.Map(endpoints);
        GetNotasConhecimentoEndpoint.Map(endpoints);
        GetValidadeTokenEndpoint.Map(endpoints);
        UpdateDataAgendamentoEndpoint.Map(endpoints);
        SendEmailAnexoEndpoint.Map(endpoints);
    }
}