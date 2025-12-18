using Nexus.Sample.MinimalApi.Common;
using Nexus.Sample.MinimalApi.Endpoints.Bancos;

namespace Nexus.Sample.MinimalApi.Endpoints;

/// <summary>
/// Agrupador de endpoints relacionados a Bancos
/// </summary>

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "OK" });

        var bankEndpoints = endpoints.MapGroup("v1")
            .WithTags("Bancos");

        // Registrar os 4 endpoints principais
        CreateBancosEndpoint.Map(bankEndpoints);
        GetBancosByIdEndpoint.Map(bankEndpoints);
        GetAllBancosEndpoint.Map(bankEndpoints);
    }

}