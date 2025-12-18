using Nexus.Sample.MinimalApi.Endpoints.Bancos;
using Nexus.Sample.MinimalApi.Endpoints.PortalAgendamento;

namespace Nexus.Sample.MinimalApi.Endpoints;

/// <summary>
/// Agrupador de endpoints relacionados a Bancos
/// </summary>

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("apis.portalagendamento");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "OK" });

        var bankEndpoints = endpoints.MapGroup("v1")
            .WithTags("Bancos");

        // Registrar os 4 endpoints principais
        CreateBancosEndpoint.Map(bankEndpoints);
        GetBancosByIdEndpoint.Map(bankEndpoints);
        GetAllBancosEndpoint.Map(bankEndpoints);

        #region Portal Agendamento

        var portalAgendamentoEndpoints = endpoints.MapGroup("v1")
        .WithTags("PortalAgendamento");

        CreateVoucherTratativaEndpoint.Map(portalAgendamentoEndpoints);
        GetClienteEndpoint.Map(portalAgendamentoEndpoints);
        GetDataAgendamentoConfirmacaoEndpoint.Map(portalAgendamentoEndpoints);
        GetDataAgendamentoPdfEndpoint.Map(portalAgendamentoEndpoints);
        GetNotasConhecimentoEndpoint.Map(portalAgendamentoEndpoints);
        GetValidadeTokenEndpoint.Map(portalAgendamentoEndpoints);
        UpdateDataAgendamentoEndpoint.Map(portalAgendamentoEndpoints);
        SendEmailAnexoEndpoint.Map(portalAgendamentoEndpoints);

        #endregion

    }

}
