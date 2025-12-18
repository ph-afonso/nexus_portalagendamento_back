using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Nexus.PortalAgendamento.Library.Infrastructure;

namespace Nexus.PortalAgendamento.MinimalApi;

/// <summary>
/// Extensões para adicionar os serviços de infraestrutura
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Adiciona os serviços de infraestrutura necessários para o funcionamento da aplicação
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Adiciona os serviços de aplicação
        services.AddApplicationInfrastructure();
        
        return services;
    }
} 