using Nexus.PortalAgendamento.MinimalApi.Endpoints.PortalAgendamento;

namespace Nexus.PortalAgendamento.MinimalApi.Endpoints;

/// <summary>
/// Classe centralizadora responsável pelo registro de todas as rotas (Endpoints) da aplicação.
/// </summary>
public static class Endpoint
{
    /// <summary>
    /// Configura e mapeia os endpoints na pipeline da aplicação.
    /// </summary>
    /// <param name="app">Instância da WebApplication.</param>
    public static void MapEndpoints(this WebApplication app)
    {
        // ==============================================================================
        // 1. Health Check (Raiz)
        // ==============================================================================
        app.MapGet("/", () => new
        {
            status = "online",
            application = "Nexus Portal Agendamento API",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            timestamp = DateTime.UtcNow
        })
        .WithTags("Health Check")
        .WithSummary("Verifica a saúde e disponibilidade da API.")
        .WithDescription("Retorna status 200 se a aplicação estiver rodando. Útil para Health Probes de infraestrutura.");

        // ==============================================================================
        // 2. Grupo Principal da API (Prefixo Global)
        // ==============================================================================
        var apiGroup = app
            .MapGroup("apis.portalagendamento")
            .MapGroup("v1");

        // ==============================================================================
        // 3. Mapeamento dos Módulos de Negócio
        // ==============================================================================
        ConfirmacaoEndpoint.Map(apiGroup);
        SolicitarAlteracaoEndpoint.Map(apiGroup);
        UploadAnexoEndpoint.Map(apiGroup);
        ConfirmarComAnexoEndpoint.Map(apiGroup);
    }
}