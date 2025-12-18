# Nexus.Framework.Vault

## Overview

Nexus.Framework.Vault is a comprehensive HashiCorp Vault integration framework for .NET applications. It provides a clean, async-first API for managing secrets, seamless integration with the .NET configuration system, health monitoring capabilities, and enterprise-grade features like retry logic and SSL configuration. Built on top of VaultSharp, it offers a production-ready solution for secure secret management in cloud-native applications.

## Key Features

- **Secret Management**: Complete CRUD operations for HashiCorp Vault secrets
- **Configuration Integration**: Seamless .NET configuration system integration
- **Health Monitoring**: ASP.NET Core health check support
- **Retry Logic**: Built-in resilience with configurable retry policies
- **SSL Configuration**: Flexible SSL certificate validation options
- **KV v2 Engine**: Optimized for HashiCorp Vault KV secrets engine version 2
- **Async Operations**: Full asynchronous support with cancellation tokens
- **Dependency Injection**: Complete DI container integration
- **Enterprise Features**: Namespace support and mount point configuration
- **Comprehensive Logging**: Detailed logging for monitoring and debugging

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Vault" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- VaultSharp 1.17.5.1
- Microsoft.Extensions.Configuration.Abstractions 9.0.6
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.6
- Microsoft.Extensions.Diagnostics.HealthChecks 9.0.6
- Microsoft.Extensions.Logging.Abstractions 9.0.6
- Microsoft.Extensions.Options 9.0.6

## Configuration

### HashiCorp Vault Setup

1. Install and configure HashiCorp Vault
2. Enable the KV v2 secrets engine
3. Create authentication tokens with appropriate policies
4. Configure SSL certificates (for production)

### Application Configuration

Add Vault settings to your `appsettings.json`:

```json
{
  "Vault": {
    "Address": "https://vault.company.com:8200",
    "Token": "your-vault-token",
    "SecretsMountPoint": "secret",
    "SecretsBasePath": "myapp",
    "TimeoutSeconds": 30,
    "MaxRetries": 3,
    "RetryIntervalMilliseconds": 1000,
    "IgnoreSslErrors": false,
    "Namespace": "development"
  }
}
```

### Service Registration

Register Vault services in your `Program.cs`:

```csharp
using Nexus.Framework.Vault;

var builder = WebApplication.CreateBuilder(args);

// Register Vault services from configuration
builder.Services.AddFrameworkVault(builder.Configuration);

// Or configure programmatically
builder.Services.AddFrameworkVault(options =>
{
    options.Address = "https://vault.company.com:8200";
    options.Token = "your-vault-token";
    options.SecretsMountPoint = "secret";
    options.SecretsBasePath = "myapp";
    options.TimeoutSeconds = 30;
    options.MaxRetries = 3;
    options.RetryIntervalMilliseconds = 1000;
    options.IgnoreSslErrors = false;
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddVaultCheck("vault", HealthStatus.Degraded, new[] { "vault", "secrets" });

var app = builder.Build();
```

## Core Interface

### IVaultService

The main interface for Vault operations:

```csharp
public interface IVaultService
{
    Task<string?> GetSecretAsync(string path, string key, CancellationToken cancellationToken = default);
    Task<IDictionary<string, string>?> GetSecretsAsync(string path, CancellationToken cancellationToken = default);
    Task SetSecretAsync(string path, string key, string value, CancellationToken cancellationToken = default);
    Task SetSecretsAsync(string path, IDictionary<string, string> secrets, CancellationToken cancellationToken = default);
    Task DeleteSecretAsync(string path, CancellationToken cancellationToken = default);
    Task<bool> IsVaultAccessibleAsync(CancellationToken cancellationToken = default);
}
```

### VaultOptions

Configuration options for Vault integration:

```csharp
public class VaultOptions
{
    public string Address { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? Namespace { get; set; }
    public string SecretsBasePath { get; set; } = string.Empty;
    public string SecretsMountPoint { get; set; } = "secret";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public int RetryIntervalMilliseconds { get; set; } = 1000;
    public bool IgnoreSslErrors { get; set; } = false;
}
```

## Usage Examples

### 1. Basic Secret Operations

```csharp
public class SecretManagementService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<SecretManagementService> _logger;
    
    public SecretManagementService(IVaultService vaultService, ILogger<SecretManagementService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<string?> GetDatabasePasswordAsync()
    {
        try
        {
            var password = await _vaultService.GetSecretAsync("database/production", "password");
            
            if (password != null)
            {
                _logger.LogInformation("Database password retrieved successfully");
                return password;
            }
            
            _logger.LogWarning("Database password not found in Vault");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve database password from Vault");
            throw;
        }
    }
    
    public async Task SetApiKeyAsync(string service, string apiKey)
    {
        try
        {
            await _vaultService.SetSecretAsync($"api-keys/{service}", "key", apiKey);
            _logger.LogInformation("API key stored successfully for service: {Service}", service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store API key for service: {Service}", service);
            throw;
        }
    }
    
    public async Task<Dictionary<string, string>> GetAllDatabaseSecretsAsync()
    {
        try
        {
            var secrets = await _vaultService.GetSecretsAsync("database/production");
            
            if (secrets != null)
            {
                _logger.LogInformation("Retrieved {Count} database secrets", secrets.Count);
                return secrets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            
            _logger.LogWarning("No database secrets found in Vault");
            return new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve database secrets from Vault");
            throw;
        }
    }
}
```

### 2. Configuration Integration

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Register Vault service
        builder.Services.AddFrameworkVault(builder.Configuration);
        
        // Add Vault to configuration pipeline
        var vaultService = builder.Services.BuildServiceProvider().GetRequiredService<IVaultService>();
        builder.Configuration.AddVault(vaultService, "myapp/config");
        
        // Now you can access Vault secrets through IConfiguration
        var app = builder.Build();
        
        // Example: Get database connection string from Vault
        var connectionString = app.Configuration["Database:ConnectionString"];
        
        app.Run();
    }
}

// Custom configuration loading
public class VaultConfigurationLoader
{
    private readonly IVaultService _vaultService;
    private readonly IConfiguration _configuration;
    
    public VaultConfigurationLoader(IVaultService vaultService, IConfiguration configuration)
    {
        _vaultService = vaultService;
        _configuration = configuration;
    }
    
    public async Task LoadConfigurationAsync()
    {
        var secrets = await _vaultService.GetSecretsAsync("myapp/config");
        
        if (secrets != null)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(secrets);
            
            var vaultConfig = configBuilder.Build();
            
            // Use vault configuration
            foreach (var kvp in secrets)
            {
                _configuration[kvp.Key] = kvp.Value;
            }
        }
    }
}
```

### 3. Connection String Management

```csharp
public class ConnectionStringService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<ConnectionStringService> _logger;
    
    public ConnectionStringService(IVaultService vaultService, ILogger<ConnectionStringService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<string> GetConnectionStringAsync(string environment)
    {
        try
        {
            var secrets = await _vaultService.GetSecretsAsync($"database/{environment}");
            
            if (secrets == null || !secrets.Any())
            {
                throw new InvalidOperationException($"No database secrets found for environment: {environment}");
            }
            
            var connectionString = BuildConnectionString(secrets);
            
            _logger.LogInformation("Connection string retrieved for environment: {Environment}", environment);
            return connectionString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get connection string for environment: {Environment}", environment);
            throw;
        }
    }
    
    private string BuildConnectionString(IDictionary<string, string> secrets)
    {
        var server = secrets.GetValueOrDefault("server", "localhost");
        var database = secrets.GetValueOrDefault("database", "defaultdb");
        var username = secrets.GetValueOrDefault("username", "");
        var password = secrets.GetValueOrDefault("password", "");
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Username and password are required for connection string");
        }
        
        return $"Server={server};Database={database};User Id={username};Password={password};";
    }
    
    public async Task UpdateConnectionStringAsync(string environment, ConnectionStringModel model)
    {
        var secrets = new Dictionary<string, string>
        {
            ["server"] = model.Server,
            ["database"] = model.Database,
            ["username"] = model.Username,
            ["password"] = model.Password
        };
        
        await _vaultService.SetSecretsAsync($"database/{environment}", secrets);
        
        _logger.LogInformation("Connection string updated for environment: {Environment}", environment);
    }
}

public class ConnectionStringModel
{
    public string Server { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

### 4. Application Secrets Management

```csharp
public class ApplicationSecretsService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<ApplicationSecretsService> _logger;
    
    public ApplicationSecretsService(IVaultService vaultService, ILogger<ApplicationSecretsService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<ApplicationSecrets> GetApplicationSecretsAsync(string applicationName)
    {
        try
        {
            var secrets = await _vaultService.GetSecretsAsync($"apps/{applicationName}");
            
            if (secrets == null)
            {
                _logger.LogWarning("No secrets found for application: {ApplicationName}", applicationName);
                return new ApplicationSecrets();
            }
            
            var applicationSecrets = new ApplicationSecrets
            {
                JwtSecret = secrets.GetValueOrDefault("jwt_secret", ""),
                EncryptionKey = secrets.GetValueOrDefault("encryption_key", ""),
                ApiKeys = secrets.Where(s => s.Key.StartsWith("api_key_"))
                    .ToDictionary(s => s.Key.Replace("api_key_", ""), s => s.Value),
                ConnectionStrings = secrets.Where(s => s.Key.StartsWith("connection_"))
                    .ToDictionary(s => s.Key.Replace("connection_", ""), s => s.Value)
            };
            
            _logger.LogInformation("Retrieved secrets for application: {ApplicationName}", applicationName);
            return applicationSecrets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve secrets for application: {ApplicationName}", applicationName);
            throw;
        }
    }
    
    public async Task UpdateApplicationSecretsAsync(string applicationName, ApplicationSecrets secrets)
    {
        try
        {
            var vaultSecrets = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(secrets.JwtSecret))
                vaultSecrets["jwt_secret"] = secrets.JwtSecret;
            
            if (!string.IsNullOrEmpty(secrets.EncryptionKey))
                vaultSecrets["encryption_key"] = secrets.EncryptionKey;
            
            foreach (var apiKey in secrets.ApiKeys)
            {
                vaultSecrets[$"api_key_{apiKey.Key}"] = apiKey.Value;
            }
            
            foreach (var connectionString in secrets.ConnectionStrings)
            {
                vaultSecrets[$"connection_{connectionString.Key}"] = connectionString.Value;
            }
            
            await _vaultService.SetSecretsAsync($"apps/{applicationName}", vaultSecrets);
            
            _logger.LogInformation("Updated secrets for application: {ApplicationName}", applicationName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update secrets for application: {ApplicationName}", applicationName);
            throw;
        }
    }
}

public class ApplicationSecrets
{
    public string JwtSecret { get; set; } = string.Empty;
    public string EncryptionKey { get; set; } = string.Empty;
    public Dictionary<string, string> ApiKeys { get; set; } = new();
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();
}
```

### 5. Secret Rotation Service

```csharp
public class SecretRotationService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<SecretRotationService> _logger;
    
    public SecretRotationService(IVaultService vaultService, ILogger<SecretRotationService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task RotateApiKeyAsync(string service)
    {
        try
        {
            var newApiKey = GenerateApiKey();
            var backupPath = $"api-keys/{service}/backup";
            var currentPath = $"api-keys/{service}";
            
            // Backup current key
            var currentKey = await _vaultService.GetSecretAsync(currentPath, "key");
            if (currentKey != null)
            {
                await _vaultService.SetSecretAsync(backupPath, "previous_key", currentKey);
                await _vaultService.SetSecretAsync(backupPath, "rotated_at", DateTime.UtcNow.ToString("O"));
            }
            
            // Set new key
            await _vaultService.SetSecretAsync(currentPath, "key", newApiKey);
            await _vaultService.SetSecretAsync(currentPath, "created_at", DateTime.UtcNow.ToString("O"));
            
            _logger.LogInformation("API key rotated successfully for service: {Service}", service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rotate API key for service: {Service}", service);
            throw;
        }
    }
    
    public async Task RotateJwtSecretAsync(string applicationName)
    {
        try
        {
            var newSecret = GenerateJwtSecret();
            var currentSecrets = await _vaultService.GetSecretsAsync($"apps/{applicationName}");
            
            if (currentSecrets != null)
            {
                // Backup current secret
                var currentSecret = currentSecrets.GetValueOrDefault("jwt_secret", "");
                if (!string.IsNullOrEmpty(currentSecret))
                {
                    await _vaultService.SetSecretAsync($"apps/{applicationName}/backup", "previous_jwt_secret", currentSecret);
                }
                
                // Update with new secret
                currentSecrets["jwt_secret"] = newSecret;
                currentSecrets["jwt_rotated_at"] = DateTime.UtcNow.ToString("O");
                
                await _vaultService.SetSecretsAsync($"apps/{applicationName}", currentSecrets);
            }
            
            _logger.LogInformation("JWT secret rotated successfully for application: {ApplicationName}", applicationName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rotate JWT secret for application: {ApplicationName}", applicationName);
            throw;
        }
    }
    
    private string GenerateApiKey()
    {
        return Guid.NewGuid().ToString("N").ToUpper();
    }
    
    private string GenerateJwtSecret()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }
}
```

### 6. Health Check Integration

```csharp
public class VaultHealthService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<VaultHealthService> _logger;
    
    public VaultHealthService(IVaultService vaultService, ILogger<VaultHealthService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckVaultHealthAsync()
    {
        try
        {
            var isAccessible = await _vaultService.IsVaultAccessibleAsync();
            
            if (isAccessible)
            {
                _logger.LogInformation("Vault health check passed");
                return HealthCheckResult.Healthy("Vault is accessible");
            }
            
            _logger.LogWarning("Vault health check failed - not accessible");
            return HealthCheckResult.Unhealthy("Vault is not accessible");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vault health check failed with exception");
            return HealthCheckResult.Unhealthy("Vault health check failed", ex);
        }
    }
    
    public async Task<VaultHealthStatus> GetDetailedHealthStatusAsync()
    {
        var healthStatus = new VaultHealthStatus();
        
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            healthStatus.IsAccessible = await _vaultService.IsVaultAccessibleAsync();
            
            stopwatch.Stop();
            healthStatus.ResponseTime = stopwatch.ElapsedMilliseconds;
            
            if (healthStatus.IsAccessible)
            {
                // Test read operation
                try
                {
                    await _vaultService.GetSecretAsync("health/check", "test");
                    healthStatus.CanRead = true;
                }
                catch
                {
                    healthStatus.CanRead = false;
                }
                
                // Test write operation
                try
                {
                    await _vaultService.SetSecretAsync("health/check", "test", DateTime.UtcNow.ToString("O"));
                    healthStatus.CanWrite = true;
                }
                catch
                {
                    healthStatus.CanWrite = false;
                }
            }
            
            healthStatus.LastChecked = DateTime.UtcNow;
            healthStatus.Status = healthStatus.IsAccessible && healthStatus.CanRead ? "Healthy" : "Unhealthy";
            
            _logger.LogInformation("Vault detailed health status: {Status}", healthStatus.Status);
            
            return healthStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get detailed Vault health status");
            
            healthStatus.Status = "Error";
            healthStatus.ErrorMessage = ex.Message;
            healthStatus.LastChecked = DateTime.UtcNow;
            
            return healthStatus;
        }
    }
}

public class VaultHealthStatus
{
    public bool IsAccessible { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public long ResponseTime { get; set; }
    public string Status { get; set; } = "Unknown";
    public string? ErrorMessage { get; set; }
    public DateTime LastChecked { get; set; }
}
```

### 7. Background Secret Refresher

```csharp
public class SecretRefreshService : BackgroundService
{
    private readonly IVaultService _vaultService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SecretRefreshService> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public SecretRefreshService(
        IVaultService vaultService,
        IConfiguration configuration,
        ILogger<SecretRefreshService> logger,
        IServiceProvider serviceProvider)
    {
        _vaultService = vaultService;
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Secret refresh service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshSecretsAsync();
                
                // Refresh every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in secret refresh service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        
        _logger.LogInformation("Secret refresh service stopped");
    }
    
    private async Task RefreshSecretsAsync()
    {
        try
        {
            var secrets = await _vaultService.GetSecretsAsync("myapp/config");
            
            if (secrets != null)
            {
                foreach (var secret in secrets)
                {
                    var key = secret.Key.Replace("__", ":");
                    _configuration[key] = secret.Value;
                }
                
                _logger.LogInformation("Refreshed {Count} secrets from Vault", secrets.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh secrets from Vault");
        }
    }
}
```

## Advanced Features

### 1. Custom Vault Client Configuration

```csharp
public class CustomVaultService : IVaultService
{
    private readonly VaultClient _vaultClient;
    private readonly VaultOptions _options;
    private readonly ILogger<CustomVaultService> _logger;
    
    public CustomVaultService(IOptions<VaultOptions> options, ILogger<CustomVaultService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        var vaultClientSettings = new VaultClientSettings(_options.Address, new TokenAuthMethod(_options.Token))
        {
            Namespace = _options.Namespace,
            PostProcessHttpClientAction = (client) =>
            {
                client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
                
                if (_options.IgnoreSslErrors)
                {
                    var handler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    
                    client = new HttpClient(handler);
                }
            }
        };
        
        _vaultClient = new VaultClient(vaultClientSettings);
    }
    
    public async Task<string?> GetSecretAsync(string path, string key, CancellationToken cancellationToken = default)
    {
        // Custom implementation with enhanced logging and error handling
        _logger.LogDebug("Getting secret from path: {Path}, key: {Key}", path, key);
        
        try
        {
            var fullPath = $"{_options.SecretsBasePath}/{path}";
            var result = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(fullPath, mountPoint: _options.SecretsMountPoint, cancellationToken: cancellationToken);
            
            if (result?.Data?.Data?.ContainsKey(key) == true)
            {
                var value = result.Data.Data[key]?.ToString();
                _logger.LogDebug("Secret retrieved successfully from path: {Path}, key: {Key}", path, key);
                return value;
            }
            
            _logger.LogWarning("Secret not found at path: {Path}, key: {Key}", path, key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get secret from path: {Path}, key: {Key}", path, key);
            throw;
        }
    }
    
    // Implement other interface methods...
}
```

### 2. Secret Caching

```csharp
public class CachedVaultService : IVaultService
{
    private readonly IVaultService _vaultService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedVaultService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    
    public CachedVaultService(IVaultService vaultService, IMemoryCache cache, ILogger<CachedVaultService> logger)
    {
        _vaultService = vaultService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<string?> GetSecretAsync(string path, string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"vault_secret_{path}_{key}";
        
        if (_cache.TryGetValue(cacheKey, out string? cachedValue))
        {
            _logger.LogDebug("Retrieved secret from cache: {Path}/{Key}", path, key);
            return cachedValue;
        }
        
        var value = await _vaultService.GetSecretAsync(path, key, cancellationToken);
        
        if (value != null)
        {
            _cache.Set(cacheKey, value, _cacheExpiration);
            _logger.LogDebug("Cached secret: {Path}/{Key}", path, key);
        }
        
        return value;
    }
    
    public async Task SetSecretAsync(string path, string key, string value, CancellationToken cancellationToken = default)
    {
        await _vaultService.SetSecretAsync(path, key, value, cancellationToken);
        
        // Invalidate cache
        var cacheKey = $"vault_secret_{path}_{key}";
        _cache.Remove(cacheKey);
        
        _logger.LogDebug("Invalidated cache for secret: {Path}/{Key}", path, key);
    }
    
    // Implement other interface methods...
}
```

### 3. Vault Policy Management

```csharp
public class VaultPolicyService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<VaultPolicyService> _logger;
    
    public VaultPolicyService(IVaultService vaultService, ILogger<VaultPolicyService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<bool> ValidateAccessAsync(string path, string operation)
    {
        try
        {
            switch (operation.ToLower())
            {
                case "read":
                    var value = await _vaultService.GetSecretAsync(path, "test");
                    return true;
                
                case "write":
                    await _vaultService.SetSecretAsync(path, "test", "validation");
                    return true;
                
                case "delete":
                    await _vaultService.DeleteSecretAsync(path);
                    return true;
                
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Access validation failed for path: {Path}, operation: {Operation}", path, operation);
            return false;
        }
    }
}
```

## Best Practices

### 1. Error Handling

```csharp
public class SafeVaultService
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<SafeVaultService> _logger;
    
    public SafeVaultService(IVaultService vaultService, ILogger<SafeVaultService> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<string?> GetSecretWithFallbackAsync(string path, string key, string? fallbackValue = null)
    {
        try
        {
            var value = await _vaultService.GetSecretAsync(path, key);
            return value ?? fallbackValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get secret from Vault, using fallback. Path: {Path}, Key: {Key}", path, key);
            return fallbackValue;
        }
    }
    
    public async Task<bool> TrySetSecretAsync(string path, string key, string value)
    {
        try
        {
            await _vaultService.SetSecretAsync(path, key, value);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set secret in Vault. Path: {Path}, Key: {Key}", path, key);
            return false;
        }
    }
}
```

### 2. Configuration Validation

```csharp
public class VaultConfigurationValidator
{
    public static void ValidateConfiguration(VaultOptions options)
    {
        if (string.IsNullOrEmpty(options.Address))
            throw new ArgumentException("Vault address is required");
        
        if (string.IsNullOrEmpty(options.Token))
            throw new ArgumentException("Vault token is required");
        
        if (!Uri.TryCreate(options.Address, UriKind.Absolute, out _))
            throw new ArgumentException("Vault address must be a valid URL");
        
        if (options.TimeoutSeconds <= 0)
            throw new ArgumentException("Timeout must be greater than 0");
        
        if (options.MaxRetries < 0)
            throw new ArgumentException("Max retries cannot be negative");
    }
}
```

### 3. Token Management

```csharp
public class VaultTokenManager
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<VaultTokenManager> _logger;
    
    public VaultTokenManager(IVaultService vaultService, ILogger<VaultTokenManager> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }
    
    public async Task<bool> ValidateTokenAsync()
    {
        try
        {
            return await _vaultService.IsVaultAccessibleAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return false;
        }
    }
    
    public async Task<TimeSpan?> GetTokenTtlAsync()
    {
        try
        {
            // Implementation depends on VaultSharp capabilities
            // This is a placeholder for token TTL checking
            return TimeSpan.FromHours(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get token TTL");
            return null;
        }
    }
}
```

## Testing

### 1. Unit Testing

```csharp
[Test]
public async Task GetSecretAsync_WithValidPath_ShouldReturnSecret()
{
    // Arrange
    var mockVaultService = new Mock<IVaultService>();
    var expectedSecret = "test-secret-value";
    
    mockVaultService
        .Setup(s => s.GetSecretAsync("test/path", "test-key", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedSecret);
    
    var service = new SecretManagementService(mockVaultService.Object, Mock.Of<ILogger<SecretManagementService>>());
    
    // Act
    var result = await service.GetSecretAsync("test/path", "test-key");
    
    // Assert
    Assert.AreEqual(expectedSecret, result);
    mockVaultService.Verify(s => s.GetSecretAsync("test/path", "test-key", It.IsAny<CancellationToken>()), Times.Once);
}
```

### 2. Integration Testing

```csharp
[Test]
public async Task VaultService_ShouldConnectToRealVault()
{
    // Arrange
    var options = new VaultOptions
    {
        Address = "http://localhost:8200",
        Token = "dev-token",
        SecretsMountPoint = "secret"
    };
    
    var services = new ServiceCollection();
    services.AddSingleton(Options.Create(options));
    services.AddLogging();
    services.AddScoped<IVaultService, VaultService>();
    
    var provider = services.BuildServiceProvider();
    var vaultService = provider.GetRequiredService<IVaultService>();
    
    // Act & Assert
    var isAccessible = await vaultService.IsVaultAccessibleAsync();
    Assert.IsTrue(isAccessible);
}
```

## Performance Considerations

### 1. Connection Pooling

```csharp
public class OptimizedVaultService : IVaultService
{
    private static readonly HttpClient SharedHttpClient = new HttpClient();
    
    // Configure VaultClient to use shared HttpClient
    // Implementation depends on VaultSharp configuration options
}
```

### 2. Batch Operations

```csharp
public class BatchVaultService
{
    private readonly IVaultService _vaultService;
    
    public BatchVaultService(IVaultService vaultService)
    {
        _vaultService = vaultService;
    }
    
    public async Task SetMultipleSecretsAsync(Dictionary<string, Dictionary<string, string>> secretsByPath)
    {
        var tasks = secretsByPath.Select(async kvp =>
        {
            await _vaultService.SetSecretsAsync(kvp.Key, kvp.Value);
        });
        
        await Task.WhenAll(tasks);
    }
}
```

## Troubleshooting

### Common Issues

1. **Connection Timeouts**
   - Increase timeout values in configuration
   - Check network connectivity to Vault server
   - Verify firewall settings

2. **Authentication Failures**
   - Verify token is valid and not expired
   - Check token policies and permissions
   - Ensure correct Vault address

3. **Secret Not Found**
   - Verify secret path exists
   - Check mount point configuration
   - Ensure proper permissions

4. **SSL Certificate Issues**
   - Configure proper SSL certificates
   - Use `IgnoreSslErrors` for development only
   - Verify certificate trust chain

### Debugging

```csharp
// Enable detailed logging
{
  "Logging": {
    "LogLevel": {
      "Nexus.Framework.Vault": "Debug",
      "VaultSharp": "Debug"
    }
  }
}
```

## Migration Guide

### From Direct VaultSharp

```csharp
// Before
var vaultClient = new VaultClient(settings);
var result = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("myapp/config");

// After
var secret = await _vaultService.GetSecretAsync("myapp/config", "database_password");
```

### From Configuration Files

```csharp
// Before
var connectionString = Configuration.GetConnectionString("Database");

// After
var connectionString = await _vaultService.GetSecretAsync("database/prod", "connection_string");
```

## Version History

- **1.1.0**: Current version with comprehensive Vault integration
- **1.0.0**: Initial release with basic Vault functionality

## Support

For issues and questions:
- Check the troubleshooting section
- Verify Vault server configuration
- Review HashiCorp Vault documentation
- Consult the framework documentation
- Contact the development team