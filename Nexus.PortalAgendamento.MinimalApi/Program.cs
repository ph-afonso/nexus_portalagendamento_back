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
using Nexus.PortalAgendamento.Library.Infrastructure.Helper;

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddNexusData(builder.Configuration);
builder.Services.AddFrameworkLogging(builder.Configuration);

if (builder.Configuration.GetValue<bool>("UseVaultCredentials", false))
{
    builder.Services.AddFrameworkVault(builder.Configuration);
}

builder.Services.AddApplicationInfrastructure();

builder.Services.AddScoped<PdfHelper>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PublicAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Simulation"))
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("Nexus Portal Agendamento API")
        .WithTheme(ScalarTheme.Saturn)
        .WithDarkMode(true)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));
}

app.UseCors("PublicAll");
app.MapEndpoints();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var response = new
        {
            Status = report.Status.ToString(),
            Environment = app.Environment.EnvironmentName,
            Checks = report.Entries.Select(e => new { Component = e.Key, Status = e.Value.Status.ToString() })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.Run();