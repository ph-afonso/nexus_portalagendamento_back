using Microsoft.Extensions.DependencyInjection;
using Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Repository;
using Nexus.Sample.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Services;
using Nexus.Sample.Library.Infrastructure.Services.Interfaces;

namespace Nexus.Sample.Library.Infrastructure;

/// <summary>
/// Extensões para registro dos serviços de infraestrutura da aplicação
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona os serviços de infraestrutura da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services)
    {
        // Adicionando o repositório e serviço de Banco
        services.AddScoped<IBancosRepository, BancosRepository>();
        services.AddScoped<IBancosService, BancosService>();
        services.AddScoped<IPortalAgendamentoRepository, PortalAgendamentoRepository>();
        services.AddScoped<IPortalAgendamentoService, PortalAgendamentoService>();

        return services;
    }
} 