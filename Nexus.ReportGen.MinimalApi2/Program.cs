using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nexus.Framework.Data;
using Nexus.Framework.Logging;
using Nexus.Framework.Vault;
using Scalar.AspNetCore;
using Serilog;
using System.Net.Mime;
using System.Text.Json;
using Nexus.ReportGen.MinimalApi;
using Nexus.ReportGen.MinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
var seqServerUrl = builder.Configuration["Logging:Seq:ServerUrl"];
var seqApiKey = builder.Configuration["Logging:Seq:ApiKey"];
var applicationName = builder.Configuration["Logging:ApplicationName"] ?? "Nexus.ReportGen.MinimalApi";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithMemoryUsage()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .Enrich.WithCorrelationIdHeader()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("ApplicationName", applicationName)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq(
        serverUrl: seqServerUrl ?? "http://192.168.10.220:5341",
        apiKey: string.IsNullOrEmpty(seqApiKey) ? null : seqApiKey)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add Nexus Framework services - TEMPORARIAMENTE COMENTADO
// builder.Services.AddNexusData(builder.Configuration);
// builder.Services.AddFrameworkLogging(builder.Configuration);

// // Verifica se deve usar credenciais do Vault
// var useVaultCredentials = builder.Configuration.GetValue<bool>("UseVaultCredentials", false);
// if (useVaultCredentials)
// {
//     // Adiciona os servi�os do Vault
//     builder.Services.AddFrameworkVault(builder.Configuration);
// }

// // Add application services
// builder.Services.AddInfrastructureServices(builder.Configuration);


// Configura��o de Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());
/*
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "sqlserver",
        tags: new[] { "database" });
    */

// Configura��o da serializa��o JSON para preservar o case das propriedades
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Rotas básicas para teste
app.MapGet("/", () => new { message = "API funcionando!", timestamp = DateTime.Now })
    .WithName("Root")
    .WithTags("Test");

app.MapGet("/test", () => Results.Ok(new { status = "OK", message = "Teste básico funcionando" }))
    .WithName("Test")
    .WithTags("Test");

// Configura��o de Health Checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            Status = report.Status.ToString(),
            HealthChecks = report.Entries.Select(e => new
            {
                Component = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description
            }),
            TotalDuration = report.TotalDuration
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

// app.MapEndpoints(); // Temporariamente comentado para teste

app.Run();
