# Nexus.Framework.Logging

## Overview

Nexus.Framework.Logging is a comprehensive logging framework built on top of Serilog, providing structured logging capabilities for enterprise applications. The framework offers multi-destination logging (console, file, Seq), automatic enrichment, safe logging helpers, and production-ready features like file rotation and centralized log management. Designed to integrate seamlessly with .NET applications, it provides both flexibility and reliability for logging scenarios.

## Key Features

- **Structured Logging**: JSON-formatted logs with rich contextual information
- **Multi-Destination Output**: Console, file, and Seq server support
- **Automatic Enrichment**: Machine name, process ID, thread ID, and application context
- **Safe Logging Helpers**: Extension-method-free logging to avoid conflicts
- **Production-Ready**: File rotation, size limits, and buffering
- **Configurable**: Flexible configuration through appsettings.json
- **Performance Optimized**: Batch processing and async operations
- **Framework Integration**: Built-in Microsoft framework log suppression
- **Centralized Management**: Seq integration for log analysis and alerting

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Logging" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- Serilog 4.3.0 (core logging framework)
- Serilog.Sinks.Console, File, Debug, Seq
- Serilog.Enrichers.Environment, Process, Thread
- Serilog.Extensions.Hosting, Configuration
- Microsoft.Extensions.Logging 9.0.6

## Configuration

### Basic Configuration

Add logging settings to your `appsettings.json`:

```json
{
  "Logging": {
    "ApplicationName": "MyApplication",
    "Environment": "Development",
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    },
    "FilePath": "logs/app.log",
    "Seq": {
      "ServerUrl": "http://localhost:5341",
      "ApiKey": "optional-api-key"
    }
  }
}
```

### Service Registration

Register logging services in your `Program.cs`:

```csharp
using Nexus.Framework.Logging;

var builder = WebApplication.CreateBuilder(args);

// Basic logging setup
builder.Services.AddFrameworkLogging(builder.Configuration);
builder.Host.ConfigureSerilog();

// Or with Seq integration
builder.Services.AddFrameworkLoggingWithSeq(builder.Configuration);
builder.Host.ConfigureSerilog();

// Or with custom Seq configuration
builder.Services.AddFrameworkLoggingWithSeq(
    builder.Configuration,
    seqServerUrl: "http://seq.company.com",
    seqApiKey: "your-api-key");
builder.Host.ConfigureSerilog();

var app = builder.Build();
```

## Core Components

### 1. ILogConfig Interface

Basic configuration contract for application context:

```csharp
public interface ILogConfig
{
    string ApplicationName { get; }
    string Environment { get; }
}

public class LogConfig : ILogConfig
{
    public string ApplicationName { get; }
    public string Environment { get; }
    
    public LogConfig(string applicationName, string environment)
    {
        ApplicationName = applicationName;
        Environment = environment;
    }
}
```

### 2. LoggingHelper Class

Safe logging methods without extension conflicts:

```csharp
public static class LoggingHelper
{
    public static void LogInformation(ILogger logger, string message, params object[] args);
    public static void LogDebug(ILogger logger, string message, params object[] args);
    public static void LogWarning(ILogger logger, string message, params object[] args);
    public static void LogError(ILogger logger, Exception exception, string message, params object[] args);
    public static void LogError(ILogger logger, string message, params object[] args);
    public static void LogCritical(ILogger logger, Exception exception, string message, params object[] args);
    public static void LogTrace(ILogger logger, string message, params object[] args);
}
```

### 3. SerilogConfigurator

Main configuration class for Serilog setup:

```csharp
public static class SerilogConfigurator
{
    public static IHostBuilder ConfigureSerilog(this IHostBuilder builder);
    private static void ConfigureLoggerConfiguration(IConfiguration configuration, LoggerConfiguration loggerConfig, ILogConfig logConfig);
}
```

## Usage Examples

### 1. Basic Logging Setup

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure logging
        builder.Services.AddFrameworkLogging(builder.Configuration);
        builder.Host.ConfigureSerilog();
        
        var app = builder.Build();
        
        // Use logging
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        LoggingHelper.LogInformation(logger, "Application started successfully");
        
        app.Run();
    }
}
```

### 2. Service with Logging

```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IProductRepository _repository;
    
    public ProductService(ILogger<ProductService> logger, IProductRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    public async Task<Product> GetProductAsync(int productId)
    {
        LoggingHelper.LogInformation(_logger, "Retrieving product with ID: {ProductId}", productId);
        
        try
        {
            var product = await _repository.GetByIdAsync(productId);
            
            if (product == null)
            {
                LoggingHelper.LogWarning(_logger, "Product not found: {ProductId}", productId);
                return null;
            }
            
            LoggingHelper.LogInformation(_logger, "Product retrieved successfully: {ProductId}", productId);
            return product;
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(_logger, ex, "Error retrieving product: {ProductId}", productId);
            throw;
        }
    }
    
    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        LoggingHelper.LogInformation(_logger, "Creating new product: {ProductName}", request.Name);
        
        try
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                CategoryId = request.CategoryId
            };
            
            await _repository.CreateAsync(product);
            
            LoggingHelper.LogInformation(_logger, "Product created successfully: {ProductId}", product.Id);
            return product;
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(_logger, ex, "Error creating product: {ProductName}", request.Name);
            throw;
        }
    }
}
```

### 3. Structured Logging with Context

```csharp
public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }
    
    public async Task ProcessOrderAsync(int orderId, int customerId)
    {
        using (_logger.BeginScope("OrderProcessing"))
        using (_logger.BeginScope("OrderId:{OrderId}", orderId))
        using (_logger.BeginScope("CustomerId:{CustomerId}", customerId))
        {
            LoggingHelper.LogInformation(_logger, "Starting order processing");
            
            try
            {
                await ValidateOrderAsync(orderId);
                await ProcessPaymentAsync(orderId);
                await UpdateInventoryAsync(orderId);
                await SendConfirmationAsync(orderId);
                
                LoggingHelper.LogInformation(_logger, "Order processed successfully");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(_logger, ex, "Order processing failed");
                throw;
            }
        }
    }
    
    private async Task ValidateOrderAsync(int orderId)
    {
        LoggingHelper.LogDebug(_logger, "Validating order");
        // Validation logic
        await Task.CompletedTask;
    }
    
    private async Task ProcessPaymentAsync(int orderId)
    {
        LoggingHelper.LogDebug(_logger, "Processing payment");
        // Payment logic
        await Task.CompletedTask;
    }
    
    private async Task UpdateInventoryAsync(int orderId)
    {
        LoggingHelper.LogDebug(_logger, "Updating inventory");
        // Inventory logic
        await Task.CompletedTask;
    }
    
    private async Task SendConfirmationAsync(int orderId)
    {
        LoggingHelper.LogDebug(_logger, "Sending confirmation");
        // Confirmation logic
        await Task.CompletedTask;
    }
}
```

### 4. Performance Logging

```csharp
public class PerformanceLoggingService
{
    private readonly ILogger<PerformanceLoggingService> _logger;
    
    public PerformanceLoggingService(ILogger<PerformanceLoggingService> logger)
    {
        _logger = logger;
    }
    
    public async Task<T> ExecuteWithLoggingAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        
        LoggingHelper.LogInformation(_logger, "Starting operation: {OperationName}", operationName);
        
        try
        {
            var result = await operation();
            
            stopwatch.Stop();
            LoggingHelper.LogInformation(_logger, 
                "Operation completed successfully: {OperationName} in {ElapsedMilliseconds}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LoggingHelper.LogError(_logger, ex, 
                "Operation failed: {OperationName} after {ElapsedMilliseconds}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
    
    public async Task<List<Product>> GetProductsWithTimingAsync()
    {
        return await ExecuteWithLoggingAsync("GetProducts", async () =>
        {
            // Simulate data retrieval
            await Task.Delay(100);
            return new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };
        });
    }
}
```

### 5. Middleware Logging

```csharp
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        
        using (_logger.BeginScope("RequestId:{RequestId}", requestId))
        {
            LoggingHelper.LogInformation(_logger, 
                "Request started: {Method} {Path} from {RemoteIp}", 
                context.Request.Method, 
                context.Request.Path, 
                context.Connection.RemoteIpAddress);
            
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                await _next(context);
                
                stopwatch.Stop();
                LoggingHelper.LogInformation(_logger, 
                    "Request completed: {Method} {Path} with {StatusCode} in {ElapsedMilliseconds}ms", 
                    context.Request.Method, 
                    context.Request.Path, 
                    context.Response.StatusCode, 
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogError(_logger, ex, 
                    "Request failed: {Method} {Path} after {ElapsedMilliseconds}ms", 
                    context.Request.Method, 
                    context.Request.Path, 
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

// Register middleware
app.UseMiddleware<RequestLoggingMiddleware>();
```

### 6. Background Service Logging

```csharp
public class DataProcessingService : BackgroundService
{
    private readonly ILogger<DataProcessingService> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public DataProcessingService(ILogger<DataProcessingService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoggingHelper.LogInformation(_logger, "Data processing service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
                
                LoggingHelper.LogDebug(_logger, "Processing data batch");
                
                var processedCount = await dataService.ProcessPendingDataAsync();
                
                if (processedCount > 0)
                {
                    LoggingHelper.LogInformation(_logger, "Processed {ProcessedCount} items", processedCount);
                }
                
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(_logger, ex, "Error in data processing service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        
        LoggingHelper.LogInformation(_logger, "Data processing service stopped");
    }
}
```

## Advanced Configuration

### 1. Environment-Specific Logging

```json
{
  "Logging": {
    "ApplicationName": "MyApp",
    "Environment": "Production",
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "MyApp": "Debug"
    },
    "FilePath": "logs/app.log",
    "Seq": {
      "ServerUrl": "http://seq.company.com",
      "ApiKey": "production-api-key"
    }
  }
}
```

### 2. Custom Log Configuration

```csharp
public class CustomLogConfig : ILogConfig
{
    public string ApplicationName { get; }
    public string Environment { get; }
    public string Version { get; }
    public string MachineName { get; }
    
    public CustomLogConfig(IConfiguration configuration)
    {
        ApplicationName = configuration["ApplicationName"] ?? "Unknown";
        Environment = configuration["Environment"] ?? "Unknown";
        Version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown";
        MachineName = System.Environment.MachineName;
    }
}

// Register custom configuration
builder.Services.AddSingleton<ILogConfig, CustomLogConfig>();
```

### 3. Conditional Logging

```csharp
public class ConditionalLoggingService
{
    private readonly ILogger<ConditionalLoggingService> _logger;
    private readonly IConfiguration _configuration;
    
    public ConditionalLoggingService(ILogger<ConditionalLoggingService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task ProcessUserActionAsync(string userId, string action)
    {
        // Only log user actions if enabled
        if (_configuration.GetValue<bool>("Logging:LogUserActions"))
        {
            LoggingHelper.LogInformation(_logger, 
                "User action: {UserId} performed {Action}", 
                userId, action);
        }
        
        // Always log critical actions
        if (action == "Delete" || action == "Modify")
        {
            LoggingHelper.LogWarning(_logger, 
                "Critical user action: {UserId} performed {Action}", 
                userId, action);
        }
        
        await Task.CompletedTask;
    }
}
```

## Log Output Examples

### Console Output

```
[14:32:15 INF] Application started successfully
[14:32:16 INF] Retrieving product with ID: 123
[14:32:16 INF] Product retrieved successfully: 123
[14:32:17 WRN] Product not found: 456
[14:32:18 ERR] Error retrieving product: 789
System.ArgumentException: Invalid product ID
   at ProductService.GetProductAsync(Int32 productId)
```

### File Output (JSON)

```json
{
  "Timestamp": "2024-01-15T14:32:15.123Z",
  "Level": "Information",
  "MessageTemplate": "Retrieving product with ID: {ProductId}",
  "Message": "Retrieving product with ID: 123",
  "Properties": {
    "ProductId": 123,
    "ApplicationName": "MyApp",
    "Environment": "Production",
    "MachineName": "WEB-01",
    "ProcessId": 1234,
    "ThreadId": 5
  }
}
```

### Seq Integration

With Seq, you can:
- Query logs: `ProductId = 123`
- Filter by level: `@Level = 'Error'`
- Search messages: `@Message like '%product%'`
- Create alerts: `@Level = 'Error' and @Exception like '%SqlException%'`

## Best Practices

### 1. Structured Logging

```csharp
// ✅ Good: Structured logging with parameters
LoggingHelper.LogInformation(logger, "Processing order {OrderId} for customer {CustomerId}", orderId, customerId);

// ❌ Bad: String concatenation
LoggingHelper.LogInformation(logger, "Processing order " + orderId + " for customer " + customerId);
```

### 2. Log Levels

```csharp
public class LogLevelExamples
{
    public void DemonstrateLogLevels(ILogger<LogLevelExamples> logger)
    {
        // Trace: Very detailed information for debugging
        LoggingHelper.LogTrace(logger, "Entering method with parameters: {Param1}, {Param2}", param1, param2);
        
        // Debug: Information useful for debugging
        LoggingHelper.LogDebug(logger, "Processing item {ItemId}", itemId);
        
        // Information: General informational messages
        LoggingHelper.LogInformation(logger, "User {UserId} logged in successfully", userId);
        
        // Warning: Potentially harmful situations
        LoggingHelper.LogWarning(logger, "Invalid operation attempted by user {UserId}", userId);
        
        // Error: Error events that might still allow the application to continue
        LoggingHelper.LogError(logger, ex, "Failed to process item {ItemId}", itemId);
        
        // Critical: Very serious error events
        LoggingHelper.LogCritical(logger, ex, "Database connection failed");
    }
}
```

### 3. Performance Considerations

```csharp
public class PerformanceOptimizedLogging
{
    private readonly ILogger<PerformanceOptimizedLogging> _logger;
    
    public PerformanceOptimizedLogging(ILogger<PerformanceOptimizedLogging> logger)
    {
        _logger = logger;
    }
    
    public void OptimizedLogging(int itemId)
    {
        // ✅ Good: Check log level before expensive operations
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var expensiveData = GetExpensiveData();
            LoggingHelper.LogDebug(_logger, "Expensive data: {Data}", expensiveData);
        }
        
        // ✅ Good: Use structured logging for better performance
        LoggingHelper.LogInformation(_logger, "Processing item {ItemId}", itemId);
        
        // ❌ Bad: String formatting in production
        LoggingHelper.LogInformation(_logger, $"Processing item {itemId}");
    }
    
    private object GetExpensiveData()
    {
        // Expensive operation
        return new { Complex = "Data" };
    }
}
```

### 4. Error Handling

```csharp
public class ErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;
    
    public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
    {
        _logger = logger;
    }
    
    public async Task<bool> ProcessDataAsync(int dataId)
    {
        try
        {
            LoggingHelper.LogInformation(_logger, "Starting data processing for {DataId}", dataId);
            
            // Process data
            await ProcessDataLogic(dataId);
            
            LoggingHelper.LogInformation(_logger, "Data processing completed for {DataId}", dataId);
            return true;
        }
        catch (ValidationException ex)
        {
            LoggingHelper.LogWarning(_logger, "Validation failed for {DataId}: {ValidationError}", dataId, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(_logger, ex, "Unexpected error processing {DataId}", dataId);
            return false;
        }
    }
    
    private async Task ProcessDataLogic(int dataId)
    {
        await Task.CompletedTask;
    }
}
```

## Testing

### 1. Unit Testing with Logging

```csharp
[Test]
public async Task ProcessOrder_ShouldLogInformation()
{
    // Arrange
    var logger = new Mock<ILogger<OrderService>>();
    var service = new OrderService(logger.Object);
    
    // Act
    await service.ProcessOrderAsync(123, 456);
    
    // Assert
    logger.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Starting order processing")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

### 2. Integration Testing

```csharp
[Test]
public async Task Application_ShouldLogToFile()
{
    // Arrange
    var tempLogFile = Path.GetTempFileName();
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Logging:FilePath"] = tempLogFile,
            ["Logging:ApplicationName"] = "TestApp"
        })
        .Build();
    
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(configuration);
    services.AddFrameworkLogging(configuration);
    
    var provider = services.BuildServiceProvider();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    
    // Act
    LoggingHelper.LogInformation(logger, "Test message");
    
    // Allow time for log to be written
    await Task.Delay(100);
    
    // Assert
    var logContent = await File.ReadAllTextAsync(tempLogFile);
    Assert.Contains("Test message", logContent);
    
    // Cleanup
    File.Delete(tempLogFile);
}
```

## Troubleshooting

### Common Issues

1. **Logs Not Appearing**
   - Check log level configuration
   - Verify file path permissions
   - Ensure Serilog is properly configured

2. **Performance Issues**
   - Review log level settings
   - Check file rotation settings
   - Consider async logging options

3. **Seq Connection Issues**
   - Verify Seq server URL
   - Check API key configuration
   - Review network connectivity

4. **Missing Context**
   - Ensure proper scope usage
   - Check enricher configuration
   - Verify structured logging parameters

### Debugging

```csharp
// Enable Serilog self-logging
Serilog.Debugging.SelfLog.Enable(Console.Error);

// Or to file
Serilog.Debugging.SelfLog.Enable(msg => File.AppendAllText("serilog-debug.log", msg));
```

## Performance Considerations

### 1. Async Logging

```csharp
// Configure async logging for better performance
.WriteTo.Async(a => a.File(
    path: logPath,
    formatter: new JsonFormatter(),
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30,
    fileSizeLimitBytes: 10 * 1024 * 1024,
    shared: true,
    flushToDiskInterval: TimeSpan.FromSeconds(10)
))
```

### 2. Log Level Optimization

```csharp
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 3. Selective Logging

```csharp
public class SelectiveLoggingService
{
    private readonly ILogger<SelectiveLoggingService> _logger;
    
    public SelectiveLoggingService(ILogger<SelectiveLoggingService> logger)
    {
        _logger = logger;
    }
    
    public void LogSelectively(string operation, object data)
    {
        // Only log expensive operations if debug is enabled
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LoggingHelper.LogDebug(_logger, "Detailed operation: {Operation} with data: {@Data}", operation, data);
        }
        
        // Always log important operations
        LoggingHelper.LogInformation(_logger, "Operation completed: {Operation}", operation);
    }
}
```

## Migration Guide

### From Microsoft.Extensions.Logging

```csharp
// Before
_logger.LogInformation("Processing item {ItemId}", itemId);

// After
LoggingHelper.LogInformation(_logger, "Processing item {ItemId}", itemId);
```

### From Serilog Direct

```csharp
// Before
Log.Information("Processing item {ItemId}", itemId);

// After
LoggingHelper.LogInformation(_logger, "Processing item {ItemId}", itemId);
```

## Version History

- **1.1.0**: Current version with comprehensive Serilog integration
- **1.0.0**: Initial release with basic logging functionality

## Support

For issues and questions:
- Check the troubleshooting section
- Review the configuration examples
- Consult the framework documentation
- Contact the development team