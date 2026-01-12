using Microsoft.Extensions.DependencyInjection;
using Nexus.PortalAgendamento.Library.Infrastructure.Helper;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Services;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

namespace Nexus.PortalAgendamento.Library.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<PdfHelper>();
        services.AddScoped<IPortalAgendamentoRepository, PortalAgendamentoRepository>();
        services.AddScoped<IPortalAgendamentoService, PortalAgendamentoService>();

        return services;
    }
}