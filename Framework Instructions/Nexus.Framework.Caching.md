# Nexus.Framework.Caching

## Overview

Nexus.Framework.Caching is a comprehensive caching framework that provides a unified interface for both in-memory and distributed caching scenarios. Built on top of Microsoft.Extensions.Caching and StackExchange.Redis, it offers seamless switching between different cache providers with consistent API and advanced features like automatic serialization, expiration management, and the cache-aside pattern.

## Key Features

- **Unified Interface**: Single `ICacheService` interface abstracts different caching implementations
- **Multiple Providers**: Support for Memory Cache and Redis distributed cache
- **Automatic Serialization**: JSON serialization/deserialization for complex objects
- **Expiration Support**: Configurable TTL (Time To Live) for cache entries
- **Cache-Aside Pattern**: Built-in `GetOrCreateAsync` method for efficient cache management
- **Error Resilience**: Comprehensive error handling with structured logging
- **Async-First**: All operations are truly asynchronous
- **Dependency Injection**: Full integration with .NET DI container
- **Fallback Strategy**: Graceful degradation when distributed cache is unavailable

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Caching" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- Microsoft.Extensions.Caching.Memory
- Microsoft.Extensions.Caching.Abstractions
- StackExchange.Redis 2.8.41
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions

## Configuration

### Basic Configuration

Add to your `appsettings.json`:

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Caching": {
    "DefaultTTL": 300,
    "UseDistributedCache": true
  }
}
```

### Service Registration

Register caching services in your `Program.cs`:

```csharp
using Nexus.Framework.Caching;

var builder = WebApplication.CreateBuilder(args);

// Flexible configuration (auto-detects Redis)
builder.Services.AddFrameworkCache(builder.Configuration, useRedis: true);

// Memory cache only
builder.Services.AddFrameworkMemoryCache();

// Redis cache with connection string
builder.Services.AddFrameworkRedisCache("localhost:6379");

// Redis cache with configuration
builder.Services.AddFrameworkRedisCache(builder.Configuration);

var app = builder.Build();
```

## Core Interface

### ICacheService

The unified interface for all caching operations:

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### 1. Basic Cache Operations

```csharp
public class ProductService
{
    private readonly ICacheService _cacheService;
    
    public ProductService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    public async Task<Product> GetProductAsync(int productId)
    {
        var cacheKey = $"product:{productId}";
        
        // Try to get from cache
        var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            return cachedProduct;
        }
        
        // If not in cache, get from database
        var product = await _repository.GetProductAsync(productId);
        
        // Store in cache for 30 minutes
        await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
        
        return product;
    }
}
```

### 2. Cache-Aside Pattern

```csharp
public class UserService
{
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _repository;
    
    public UserService(ICacheService cacheService, IUserRepository repository)
    {
        _cacheService = cacheService;
        _repository = repository;
    }
    
    public async Task<User> GetUserAsync(int userId)
    {
        var cacheKey = $"user:{userId}";
        
        // Cache-aside pattern with automatic fallback
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () => await _repository.GetUserAsync(userId),
            TimeSpan.FromHours(1)
        );
    }
    
    public async Task<List<User>> GetActiveUsersAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            "users:active",
            async () => await _repository.GetActiveUsersAsync(),
            TimeSpan.FromMinutes(15)
        );
    }
}
```

### 3. Complex Object Caching

```csharp
public class OrderService
{
    private readonly ICacheService _cacheService;
    
    public OrderService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    public async Task<OrderSummary> GetOrderSummaryAsync(int customerId)
    {
        var cacheKey = $"order:summary:{customerId}";
        
        var summary = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () => new OrderSummary
            {
                CustomerId = customerId,
                TotalOrders = await _repository.GetOrderCountAsync(customerId),
                TotalAmount = await _repository.GetTotalAmountAsync(customerId),
                LastOrderDate = await _repository.GetLastOrderDateAsync(customerId),
                RecentOrders = await _repository.GetRecentOrdersAsync(customerId, 10)
            },
            TimeSpan.FromMinutes(20)
        );
        
        return summary;
    }
}
```

### 4. Cache Invalidation

```csharp
public class ProductService
{
    private readonly ICacheService _cacheService;
    
    public async Task UpdateProductAsync(Product product)
    {
        // Update in repository
        await _repository.UpdateProductAsync(product);
        
        // Invalidate cache
        await _cacheService.RemoveAsync($"product:{product.Id}");
        
        // Invalidate related caches
        await _cacheService.RemoveAsync($"products:category:{product.CategoryId}");
        await _cacheService.RemoveAsync("products:featured");
    }
    
    public async Task InvalidateProductCacheAsync(int productId)
    {
        var cacheKey = $"product:{productId}";
        
        // Check if exists before removing
        if (await _cacheService.ExistsAsync(cacheKey))
        {
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
}
```

### 5. Batch Operations

```csharp
public class CacheManager
{
    private readonly ICacheService _cacheService;
    
    public CacheManager(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    public async Task CacheMultipleProductsAsync(List<Product> products)
    {
        var tasks = products.Select(async product =>
        {
            var cacheKey = $"product:{product.Id}";
            await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromHours(2));
        });
        
        await Task.WhenAll(tasks);
    }
    
    public async Task<List<Product>> GetMultipleProductsAsync(List<int> productIds)
    {
        var tasks = productIds.Select(async id =>
        {
            var cacheKey = $"product:{id}";
            return await _cacheService.GetAsync<Product>(cacheKey);
        });
        
        var results = await Task.WhenAll(tasks);
        return results.Where(p => p != null).ToList();
    }
}
```

### 6. Configuration-Based Caching

```csharp
public class ConfigurableCacheService
{
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;
    
    public ConfigurableCacheService(ICacheService cacheService, IConfiguration configuration)
    {
        _cacheService = cacheService;
        _configuration = configuration;
    }
    
    public async Task<T> GetWithConfigurableExpirationAsync<T>(string key, Func<Task<T>> factory)
    {
        var defaultTTL = _configuration.GetValue<int>("Caching:DefaultTTL", 300);
        var expiration = TimeSpan.FromSeconds(defaultTTL);
        
        return await _cacheService.GetOrCreateAsync(key, factory, expiration);
    }
}
```

## Implementation Details

### Memory Cache Implementation

```csharp
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return _memoryCache.TryGetValue(key, out var value) ? (T?)value : default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            return default;
        }
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            }
            
            _memoryCache.Set(key, value, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
        }
    }
}
```

### Redis Cache Implementation

```csharp
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Redis cache key {Key}", key);
            return default;
        }
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await _database.StringSetAsync(key, serializedValue, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting Redis cache key {Key}", key);
        }
    }
}
```

## Advanced Configuration

### Custom Serialization

```csharp
public class CustomRedisCacheService : ICacheService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        await _database.StringSetAsync(key, serializedValue, expiration);
    }
}
```

### Cache Warming

```csharp
public class CacheWarmupService : BackgroundService
{
    private readonly ICacheService _cacheService;
    private readonly IServiceProvider _serviceProvider;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await WarmupCacheAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task WarmupCacheAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        
        // Warm up frequently accessed data
        var featuredProducts = await repository.GetFeaturedProductsAsync();
        await _cacheService.SetAsync("products:featured", featuredProducts, TimeSpan.FromHours(4));
        
        var categories = await repository.GetCategoriesAsync();
        await _cacheService.SetAsync("categories:all", categories, TimeSpan.FromHours(8));
    }
}
```

## Best Practices

### 1. Cache Key Naming

```csharp
// ✅ Good: Structured, hierarchical naming
public static class CacheKeys
{
    public static string UserById(int id) => $"user:id:{id}";
    public static string UserByEmail(string email) => $"user:email:{email}";
    public static string ProductsByCategory(int categoryId) => $"products:category:{categoryId}";
    public static string OrdersByCustomer(int customerId) => $"orders:customer:{customerId}";
}

// ❌ Bad: Unstructured naming
var key = "user" + userId;
var key = "prod_cat_" + categoryId;
```

### 2. Expiration Strategy

```csharp
public static class CacheExpirations
{
    public static readonly TimeSpan ShortTerm = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan MediumTerm = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan LongTerm = TimeSpan.FromHours(2);
    public static readonly TimeSpan VeryLongTerm = TimeSpan.FromDays(1);
}

// Usage
await _cacheService.SetAsync(key, value, CacheExpirations.MediumTerm);
```

### 3. Error Handling

```csharp
public class SafeCacheService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<SafeCacheService> _logger;
    
    public async Task<T> GetWithFallbackAsync<T>(string key, Func<Task<T>> fallback, TimeSpan? expiration = null)
    {
        try
        {
            // Try cache first
            var cached = await _cacheService.GetAsync<T>(key);
            if (cached != null) return cached;
            
            // Execute fallback
            var result = await fallback();
            
            // Store in cache for next time
            await _cacheService.SetAsync(key, result, expiration);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache operation failed for key {Key}, executing fallback", key);
            return await fallback();
        }
    }
}
```

### 4. Cache Invalidation Patterns

```csharp
public class SmartCacheService
{
    private readonly ICacheService _cacheService;
    
    public async Task InvalidateRelatedCacheAsync(string entityType, int entityId)
    {
        var patterns = entityType switch
        {
            "Product" => new[]
            {
                $"product:{entityId}",
                $"products:category:*",
                "products:featured"
            },
            "User" => new[]
            {
                $"user:{entityId}",
                $"users:active"
            },
            _ => Array.Empty<string>()
        };
        
        foreach (var pattern in patterns)
        {
            if (pattern.Contains("*"))
            {
                // Handle pattern-based invalidation
                await InvalidateByPatternAsync(pattern);
            }
            else
            {
                await _cacheService.RemoveAsync(pattern);
            }
        }
    }
}
```

## Performance Considerations

### 1. Memory Usage

```csharp
// Monitor memory cache size
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Limit number of entries
    options.CompactionPercentage = 0.75; // Compact when 75% full
});
```

### 2. Serialization Performance

```csharp
// Use efficient serialization options
private static readonly JsonSerializerOptions SerializerOptions = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true
};
```

### 3. Connection Pooling

```csharp
// Configure Redis connection pooling
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("Redis");
    
    return ConnectionMultiplexer.Connect(connectionString);
});
```

## Monitoring and Diagnostics

### 1. Cache Metrics

```csharp
public class CacheMetricsService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheMetricsService> _logger;
    
    public async Task<T> GetWithMetricsAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await _cacheService.GetOrCreateAsync(key, factory, expiration);
            
            _logger.LogInformation("Cache operation completed in {ElapsedMs}ms for key {Key}", 
                stopwatch.ElapsedMilliseconds, key);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache operation failed after {ElapsedMs}ms for key {Key}", 
                stopwatch.ElapsedMilliseconds, key);
            throw;
        }
    }
}
```

### 2. Health Checks

```csharp
public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var testKey = "health:check:" + Guid.NewGuid();
            var testValue = "test";
            
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(30), cancellationToken);
            var retrieved = await _cacheService.GetAsync<string>(testKey, cancellationToken);
            await _cacheService.RemoveAsync(testKey, cancellationToken);
            
            return retrieved == testValue 
                ? HealthCheckResult.Healthy("Cache is working correctly")
                : HealthCheckResult.Unhealthy("Cache read/write test failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cache health check failed", ex);
        }
    }
}

// Register health check
builder.Services.AddHealthChecks()
    .AddCheck<CacheHealthCheck>("cache");
```

## Migration Guide

### From IMemoryCache

```csharp
// Before
_memoryCache.Set("key", value, TimeSpan.FromMinutes(30));
var result = _memoryCache.Get<MyType>("key");

// After
await _cacheService.SetAsync("key", value, TimeSpan.FromMinutes(30));
var result = await _cacheService.GetAsync<MyType>("key");
```

### From IDistributedCache

```csharp
// Before
var json = await _distributedCache.GetStringAsync("key");
var value = JsonSerializer.Deserialize<MyType>(json);

// After
var value = await _cacheService.GetAsync<MyType>("key");
```

## Troubleshooting

### Common Issues

1. **Redis Connection Issues**
   - Check connection string format
   - Verify Redis server is running
   - Review firewall/network settings

2. **Serialization Errors**
   - Ensure objects are serializable
   - Check for circular references
   - Verify type compatibility

3. **Memory Issues**
   - Monitor memory cache size limits
   - Check expiration policies
   - Review object sizes being cached

### Debugging

```csharp
// Enable detailed logging
{
  "Logging": {
    "LogLevel": {
      "Nexus.Framework.Caching": "Debug"
    }
  }
}
```

## Version History

- **1.1.0**: Current version with unified interface and Redis support
- **1.0.0**: Initial release with basic caching functionality

## Support

For issues and questions:
- Check the troubleshooting section
- Review the configuration examples
- Consult the framework documentation
- Contact the development team