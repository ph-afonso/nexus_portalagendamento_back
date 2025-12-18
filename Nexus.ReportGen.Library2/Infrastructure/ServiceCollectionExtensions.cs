using Microsoft.Extensions.DependencyInjection;

namespace Nexus.ReportGen.Library.Infrastructure;

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
        // Registrar todos os repositórios
        services.AddScoped<Repository.Interfaces.IRelatorioCategoriaRepository, Repository.RelatorioCategoriaRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioDestinoTipoRepository, Repository.RelatorioDestinoTipoRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioLogRepository, Repository.RelatorioLogRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioModeloRepository, Repository.RelatorioModeloRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioModeloDestinoRepository, Repository.RelatorioModeloDestinoRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioModeloParametroRepository, Repository.RelatorioModeloParametroRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioModeloPermissaoRepository, Repository.RelatorioModeloPermissaoRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioModeloSaidaRepository, Repository.RelatorioModeloSaidaRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioParametroOpcaoRepository, Repository.RelatorioParametroOpcaoRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioSolicitacaoRepository, Repository.RelatorioSolicitacaoRepository>();
        services.AddScoped<Repository.Interfaces.IRelatorioSolicitacaoDestinoRepository, Repository.RelatorioSolicitacaoDestinoRepository>();
        services.AddScoped<Repository.Interfaces.IStatusProcessamentoRepository, Repository.StatusProcessamentoRepository>();
        services.AddScoped<Repository.Interfaces.ITipoParametroRepository, Repository.TipoParametroRepository>();

        // Registrar todos os serviços
        services.AddScoped<Services.Interfaces.IRelatorioCategoriaService, Services.RelatorioCategoriaService>();
        services.AddScoped<Services.Interfaces.IRelatorioDestinoTipoService, Services.RelatorioDestinoTipoService>();
        services.AddScoped<Services.Interfaces.IRelatorioLogService, Services.RelatorioLogService>();
        services.AddScoped<Services.Interfaces.IRelatorioModeloService, Services.RelatorioModeloService>();
        services.AddScoped<Services.Interfaces.IRelatorioModeloDestinoService, Services.RelatorioModeloDestinoService>();
        services.AddScoped<Services.Interfaces.IRelatorioModeloParametroService, Services.RelatorioModeloParametroService>();
        services.AddScoped<Services.Interfaces.IRelatorioModeloPermissaoService, Services.RelatorioModeloPermissaoService>();
        services.AddScoped<Services.Interfaces.IRelatorioModeloSaidaService, Services.RelatorioModeloSaidaService>();
        services.AddScoped<Services.Interfaces.IRelatorioParametroOpcaoService, Services.RelatorioParametroOpcaoService>();
        services.AddScoped<Services.Interfaces.IRelatorioSolicitacaoService, Services.RelatorioSolicitacaoService>();
        services.AddScoped<Services.Interfaces.IRelatorioSolicitacaoDestinoService, Services.RelatorioSolicitacaoDestinoService>();
        services.AddScoped<Services.Interfaces.IStatusProcessamentoService, Services.StatusProcessamentoService>();
        services.AddScoped<Services.Interfaces.ITipoParametroService, Services.TipoParametroService>();
        
        return services;
    }
} 