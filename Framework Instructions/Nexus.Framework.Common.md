# Nexus.Framework.Common

## Overview

Nexus.Framework.Common is the foundational library of the Nexus Framework, providing essential result patterns, message handling, and shared utilities used throughout the entire framework ecosystem. This library implements a comprehensive result pattern that ensures consistent return structures across all framework operations, with built-in support for success/failure states, typed data payloads, and user-friendly messaging.

## Key Features

- **Result Pattern**: Comprehensive result pattern with `NexusResult<T>` for consistent operation returns
- **Message System**: Structured message handling with `MessageDetails` for user feedback
- **Type Safety**: Strong typing support through generics and nullable reference types
- **Factory Methods**: Static factory methods for common result creation scenarios
- **Database Integration**: Built-in support for processing database return messages
- **Extensibility**: Properties dictionary for custom metadata
- **Standardized Codes**: Consistent status codes across the framework
- **Fluent Interface**: Method chaining for result configuration

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Common" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.6

## Configuration

### Service Registration

Register common services in your `Program.cs`:

```csharp
using Nexus.Framework.Common;

var builder = WebApplication.CreateBuilder(args);

// Register common framework services
builder.Services.AddFrameworkCommon();

var app = builder.Build();
```

## Core Components

### 1. NexusResult Pattern

The framework implements a comprehensive result pattern with two main classes:

#### NexusResult<T> (Generic Result Class)

```csharp
public class NexusResult<T>
{
    public int Code { get; set; }
    public bool IsSuccess => Code == 77700;
    public T? ResultData { get; set; }
    public List<MessageDetails> Messages { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();
}
```

#### NexusResult (Static Factory Class)

```csharp
public static class NexusResult
{
    public static NexusResult<T> Create<T>(int statusCode);
    public static NexusResult<T> Create<T>(int code, T data);
    public static NexusResult<T> Create<T>(int code, T data, List<MessageDetails> messages);
    public static NexusResult<object> CreateSuccess();
    public static NexusResult<T> CreateSuccess<T>(T data);
    public static NexusResult<object> CreateFailure();
}
```

### 2. Standard Status Codes

The framework defines standardized status codes:

```csharp
public const int SUCCESS_CODE = 77700;
public const int FAILURE_CODE = 77701;
public const string SUCCESS_MESSAGE = "Operação realizada com sucesso.";
public const string FAILURE_MESSAGE = "Operação não realizada.";
```

### 3. Message System

```csharp
public class MessageDetails
{
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public MessageDetails() { }
    public MessageDetails(string description) { Description = description; }
    public MessageDetails(string title, string description) { Title = title; Description = description; }
}
```

## Usage Examples

### 1. Basic Result Creation

```csharp
public class UserService
{
    public async Task<NexusResult<User>> GetUserAsync(int userId)
    {
        var user = await _repository.GetUserAsync(userId);
        
        if (user == null)
        {
            return NexusResult.Create<User>(77701, null, "Usuário não encontrado");
        }
        
        return NexusResult.CreateSuccess(user);
    }
    
    public async Task<NexusResult<User>> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email
            };
            
            await _repository.CreateUserAsync(user);
            return NexusResult.CreateSuccess(user);
        }
        catch (Exception ex)
        {
            return NexusResult.Create<User>(77701, null, $"Erro ao criar usuário: {ex.Message}");
        }
    }
}
```

### 2. Working with Messages

```csharp
public class ProductService
{
    public async Task<NexusResult<Product>> ValidateAndCreateProductAsync(CreateProductRequest request)
    {
        var result = new NexusResult<Product>();
        
        // Validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            result.AddFailureMessage("Nome do produto é obrigatório");
        }
        
        if (request.Price <= 0)
        {
            result.AddFailureMessage("Preço deve ser maior que zero");
        }
        
        if (result.Messages.Any(m => m.Description.Contains("obrigatório") || m.Description.Contains("deve ser")))
        {
            result.Code = 77701;
            return result;
        }
        
        // Create product
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };
        
        await _repository.CreateProductAsync(product);
        
        result.Code = 77700;
        result.ResultData = product;
        result.AddDefaultSuccessMessage();
        
        return result;
    }
}
```

### 3. Database Message Processing

```csharp
public class OrderService
{
    public async Task<NexusResult<Order>> ProcessOrderAsync(ProcessOrderRequest request)
    {
        var result = new NexusResult<Order>();
        
        // Call stored procedure that returns messages separated by '^'
        var dbResult = await _repository.ProcessOrderAsync(request);
        
        // Process database messages
        if (!string.IsNullOrEmpty(dbResult.Messages))
        {
            result.ProcessMessageString(dbResult.Messages, '^');
        }
        
        result.Code = dbResult.ReturnCode;
        result.ResultData = dbResult.OrderData;
        
        return result;
    }
}
```

### 4. Custom Properties and Metadata

```csharp
public class ReportService
{
    public async Task<NexusResult<ReportData>> GenerateReportAsync(ReportRequest request)
    {
        var reportData = await _repository.GenerateReportAsync(request);
        
        var result = NexusResult.CreateSuccess(reportData);
        
        // Add custom properties
        result.Properties["GeneratedAt"] = DateTime.UtcNow;
        result.Properties["GeneratedBy"] = request.UserId;
        result.Properties["ReportType"] = request.Type;
        result.Properties["RowCount"] = reportData.Rows.Count;
        
        return result;
    }
}
```

### 5. API Controller Integration

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    
    public UsersController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<NexusResult<User>>> GetUser(int id)
    {
        var result = await _userService.GetUserAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<NexusResult<User>>> CreateUser(CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetUser), new { id = result.ResultData.Id }, result);
        }
        
        return BadRequest(result);
    }
}
```

### 6. Service Layer Patterns

```csharp
public class ProductService
{
    private readonly IProductRepository _repository;
    
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<NexusResult<List<Product>>> GetProductsAsync(ProductFilter filter)
    {
        try
        {
            var products = await _repository.GetProductsAsync(filter);
            
            var result = NexusResult.CreateSuccess(products);
            result.Properties["TotalCount"] = products.Count;
            result.Properties["FilterApplied"] = filter.HasFilters;
            
            return result;
        }
        catch (Exception ex)
        {
            return NexusResult.Create<List<Product>>(77701, null, $"Erro ao buscar produtos: {ex.Message}");
        }
    }
    
    public async Task<NexusResult<Product>> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var result = new NexusResult<Product>();
        
        // Get existing product
        var existingProduct = await _repository.GetProductAsync(id);
        if (existingProduct == null)
        {
            result.Code = 77701;
            result.AddFailureMessage("Produto não encontrado");
            return result;
        }
        
        // Update product
        existingProduct.Name = request.Name;
        existingProduct.Price = request.Price;
        existingProduct.UpdatedAt = DateTime.UtcNow;
        
        await _repository.UpdateProductAsync(existingProduct);
        
        result.Code = 77700;
        result.ResultData = existingProduct;
        result.AddDefaultSuccessMessage();
        
        return result;
    }
}
```

### 7. Extension Methods

```csharp
public static class NexusResultExtensions
{
    public static NexusResult<T> AddValidationMessage<T>(this NexusResult<T> result, string field, string message)
    {
        result.Messages.Add(new MessageDetails($"Validação - {field}", message));
        return result;
    }
    
    public static NexusResult<T> AddInfoMessage<T>(this NexusResult<T> result, string message)
    {
        result.Messages.Add(new MessageDetails("Informação", message));
        return result;
    }
    
    public static NexusResult<T> AddWarningMessage<T>(this NexusResult<T> result, string message)
    {
        result.Messages.Add(new MessageDetails("Aviso", message));
        return result;
    }
    
    public static bool HasErrors<T>(this NexusResult<T> result)
    {
        return !result.IsSuccess || result.Messages.Any(m => m.Title?.Contains("Erro") == true);
    }
}

// Usage
var result = new NexusResult<User>();
result.AddValidationMessage("Email", "Email é obrigatório")
      .AddValidationMessage("Name", "Nome deve ter pelo menos 3 caracteres");
```

## Advanced Features

### 1. Fluent Result Building

```csharp
public class OrderService
{
    public async Task<NexusResult<Order>> CreateOrderAsync(CreateOrderRequest request)
    {
        var result = new NexusResult<Order>();
        
        // Fluent validation and building
        return await Task.FromResult(result)
            .ContinueWith(async t => 
            {
                var r = t.Result;
                
                // Validation
                if (request.Items.Count == 0)
                {
                    r.AddFailureMessage("Pedido deve ter pelo menos um item");
                }
                
                if (request.CustomerId <= 0)
                {
                    r.AddFailureMessage("Cliente é obrigatório");
                }
                
                // If validation failed, return early
                if (r.Messages.Any())
                {
                    r.Code = 77701;
                    return r;
                }
                
                // Create order
                var order = new Order
                {
                    CustomerId = request.CustomerId,
                    Items = request.Items,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _repository.CreateOrderAsync(order);
                
                r.Code = 77700;
                r.ResultData = order;
                r.AddDefaultSuccessMessage();
                
                return r;
            })
            .Unwrap();
    }
}
```

### 2. Result Transformation

```csharp
public static class ResultTransformations
{
    public static NexusResult<TOut> Transform<TIn, TOut>(this NexusResult<TIn> source, Func<TIn, TOut> transformer)
    {
        if (!source.IsSuccess)
        {
            return NexusResult.Create<TOut>(source.Code, default, source.Messages);
        }
        
        try
        {
            var transformedData = transformer(source.ResultData);
            return NexusResult.CreateSuccess(transformedData);
        }
        catch (Exception ex)
        {
            return NexusResult.Create<TOut>(77701, default, $"Erro na transformação: {ex.Message}");
        }
    }
}

// Usage
var userResult = await _userService.GetUserAsync(userId);
var userDtoResult = userResult.Transform(user => new UserDto
{
    Id = user.Id,
    Name = user.Name,
    Email = user.Email
});
```

### 3. Result Aggregation

```csharp
public class CompositeService
{
    public async Task<NexusResult<OrderSummary>> GetOrderSummaryAsync(int orderId)
    {
        var orderResult = await _orderService.GetOrderAsync(orderId);
        var customerResult = await _customerService.GetCustomerAsync(orderResult.ResultData?.CustomerId ?? 0);
        var itemsResult = await _itemService.GetOrderItemsAsync(orderId);
        
        // Aggregate results
        var aggregatedResult = new NexusResult<OrderSummary>();
        
        if (!orderResult.IsSuccess)
        {
            aggregatedResult.Messages.AddRange(orderResult.Messages);
        }
        
        if (!customerResult.IsSuccess)
        {
            aggregatedResult.Messages.AddRange(customerResult.Messages);
        }
        
        if (!itemsResult.IsSuccess)
        {
            aggregatedResult.Messages.AddRange(itemsResult.Messages);
        }
        
        if (aggregatedResult.Messages.Any())
        {
            aggregatedResult.Code = 77701;
            return aggregatedResult;
        }
        
        // Create summary
        var summary = new OrderSummary
        {
            Order = orderResult.ResultData,
            Customer = customerResult.ResultData,
            Items = itemsResult.ResultData
        };
        
        aggregatedResult.Code = 77700;
        aggregatedResult.ResultData = summary;
        aggregatedResult.AddDefaultSuccessMessage();
        
        return aggregatedResult;
    }
}
```

## Best Practices

### 1. Consistent Status Codes

```csharp
// ✅ Good: Use framework constants
public static class StatusCodes
{
    public const int SUCCESS = 77700;
    public const int GENERAL_FAILURE = 77701;
    public const int VALIDATION_ERROR = 77702;
    public const int NOT_FOUND = 77703;
    public const int UNAUTHORIZED = 77704;
    public const int FORBIDDEN = 77705;
}

// ❌ Bad: Magic numbers
var result = NexusResult.Create<User>(500, null, "Error");
```

### 2. Meaningful Messages

```csharp
// ✅ Good: Descriptive messages
result.AddFailureMessage("O email informado já está em uso por outro usuário");

// ❌ Bad: Generic messages
result.AddFailureMessage("Error");
```

### 3. Proper Error Handling

```csharp
public class ServiceBase
{
    protected NexusResult<T> HandleException<T>(Exception ex, string operation)
    {
        _logger.LogError(ex, "Erro durante {Operation}", operation);
        
        var result = NexusResult.Create<T>(77701, default);
        result.AddFailureMessage($"Erro interno durante {operation}");
        
        return result;
    }
}
```

### 4. Result Validation

```csharp
public static class ResultValidation
{
    public static bool IsValid<T>(this NexusResult<T> result)
    {
        return result.IsSuccess && result.ResultData != null;
    }
    
    public static bool HasData<T>(this NexusResult<T> result)
    {
        return result.ResultData != null;
    }
    
    public static bool HasMessages<T>(this NexusResult<T> result)
    {
        return result.Messages.Any();
    }
}
```

## Performance Considerations

### 1. Message Creation

```csharp
// ✅ Good: Reuse message objects
private static readonly MessageDetails SuccessMessage = new("Operação realizada com sucesso.");

// ❌ Bad: Create new messages every time
result.Messages.Add(new MessageDetails("Operação realizada com sucesso."));
```

### 2. Property Dictionary

```csharp
// ✅ Good: Initialize when needed
if (result.Properties == null)
{
    result.Properties = new Dictionary<string, object>();
}

// ❌ Bad: Always initialize
result.Properties = new Dictionary<string, object>();
```

## Integration with Other Framework Components

### 1. Data Layer Integration

```csharp
public class RepositoryBase
{
    protected NexusResult<T> ProcessDbResult<T>(DbExecutionResult dbResult, T data = default)
    {
        var result = new NexusResult<T>
        {
            Code = dbResult.ReturnCode,
            ResultData = data
        };
        
        if (dbResult.OutputParameters.ContainsKey("Messages"))
        {
            result.ProcessMessageString(dbResult.OutputParameters["Messages"].ToString());
        }
        
        return result;
    }
}
```

### 2. Caching Integration

```csharp
public class CachedService
{
    public async Task<NexusResult<T>> GetCachedDataAsync<T>(string key, Func<Task<NexusResult<T>>> factory)
    {
        var cached = await _cache.GetAsync<NexusResult<T>>(key);
        if (cached != null && cached.IsSuccess)
        {
            return cached;
        }
        
        var result = await factory();
        if (result.IsSuccess)
        {
            await _cache.SetAsync(key, result, TimeSpan.FromMinutes(30));
        }
        
        return result;
    }
}
```

## Testing

### 1. Unit Testing Results

```csharp
[Test]
public async Task CreateUser_WithValidData_ShouldReturnSuccess()
{
    // Arrange
    var request = new CreateUserRequest { Name = "Test User", Email = "test@example.com" };
    
    // Act
    var result = await _userService.CreateUserAsync(request);
    
    // Assert
    Assert.IsTrue(result.IsSuccess);
    Assert.IsNotNull(result.ResultData);
    Assert.AreEqual(77700, result.Code);
    Assert.IsTrue(result.Messages.Any(m => m.Description.Contains("sucesso")));
}

[Test]
public async Task CreateUser_WithInvalidData_ShouldReturnFailure()
{
    // Arrange
    var request = new CreateUserRequest { Name = "", Email = "invalid-email" };
    
    // Act
    var result = await _userService.CreateUserAsync(request);
    
    // Assert
    Assert.IsFalse(result.IsSuccess);
    Assert.IsNull(result.ResultData);
    Assert.AreEqual(77701, result.Code);
    Assert.IsTrue(result.Messages.Any());
}
```

### 2. Integration Testing

```csharp
[Test]
public async Task GetUser_ExistingUser_ShouldReturnUserData()
{
    // Arrange
    var userId = 1;
    
    // Act
    var result = await _userService.GetUserAsync(userId);
    
    // Assert
    Assert.IsTrue(result.IsSuccess);
    Assert.IsNotNull(result.ResultData);
    Assert.AreEqual(userId, result.ResultData.Id);
}
```

## Migration Guide

### From Direct Returns

```csharp
// Before
public async Task<User> GetUserAsync(int id)
{
    return await _repository.GetUserAsync(id);
}

// After
public async Task<NexusResult<User>> GetUserAsync(int id)
{
    var user = await _repository.GetUserAsync(id);
    return user != null 
        ? NexusResult.CreateSuccess(user)
        : NexusResult.Create<User>(77701, null, "Usuário não encontrado");
}
```

### From Custom Result Classes

```csharp
// Before
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}

// After
public async Task<NexusResult<T>> GetDataAsync()
{
    var result = NexusResult.CreateSuccess(data);
    result.AddDefaultSuccessMessage();
    return result;
}
```

## Troubleshooting

### Common Issues

1. **Messages Not Appearing**
   - Check if `AddDefaultSuccessMessage()` or `AddFailureMessage()` is called
   - Verify message processing with `ProcessMessageString()`

2. **Status Code Confusion**
   - Use framework constants: `77700` for success, `77701` for failure
   - Check `IsSuccess` property instead of comparing codes directly

3. **Null Reference Exceptions**
   - Enable nullable reference types in project
   - Initialize collections in constructors

## Version History

- **1.1.0**: Current version with comprehensive result pattern
- **1.0.0**: Initial release with basic result structures

## Support

For issues and questions:
- Review the usage examples
- Check the best practices section
- Consult the framework documentation
- Contact the development team