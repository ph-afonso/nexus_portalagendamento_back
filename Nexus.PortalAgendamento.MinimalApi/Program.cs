using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nexus.Framework.Data;
using Nexus.Framework.Logging;
using Nexus.Framework.Vault;
using Nexus.PortalAgendamento.Library.Infrastructure;
using Nexus.PortalAgendamento.MinimalApi.Endpoints; 
using Scalar.AspNetCore;
using Serilog;
using System.Net.Mime;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração de Logs (Serilog)
var applicationName = builder.Configuration["Logging:ApplicationName"] ?? "Nexus.PortalAgendamento.MinimalApi";
var seqServerUrl = builder.Configuration["Logging:Seq:ServerUrl"];
var seqApiKey = builder.Configuration["Logging:Seq:ApiKey"];

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", applicationName)
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.Seq(serverUrl: seqServerUrl ?? "http://localhost:5341", apiKey: seqApiKey)
    .CreateLogger();

builder.Host.UseSerilog();

// 2. Serviços da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// 3. Nexus Framework e Infraestrutura
builder.Services.AddNexusData(builder.Configuration);
builder.Services.AddFrameworkLogging(builder.Configuration);

if (builder.Configuration.GetValue<bool>("UseVaultCredentials", false))
{
    builder.Services.AddFrameworkVault(builder.Configuration);
}

// Injeta os serviços do Portal (Bancos e Agendamento)
builder.Services.AddApplicationInfrastructure();

// 4. Configuração JSON (Remove conversão para camelCase se o banco usa PascalCase)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PublicAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// 6. Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Pipeline de Execução
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("Nexus Portal Agendamento API")
        .WithTheme(ScalarTheme.Saturn)
        .WithDarkMode());
}

app.UseCors("PublicAll");

// Mapeamento automático dos Endpoints
app.MapEndpoints();

// Endpoint de Health Check
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var response = new
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(e => new { Component = e.Key, Status = e.Value.Status.ToString() })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.Run();