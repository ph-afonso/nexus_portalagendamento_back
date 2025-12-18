# Nexus.Framework.Data

## Overview

Nexus.Framework.Data is a sophisticated data access framework built on top of Dapper that implements advanced patterns including Repository Pattern, CQRS (Command Query Responsibility Segregation), Unit of Work, and Hybrid Connection Management. This framework is designed for enterprise applications requiring high performance, security, and maintainability.

## Key Features

- **Repository Pattern with CQRS**: Automatic routing of read/write operations based on stored procedure naming conventions
- **Hybrid Connection Management**: Supports both HashiCorp Vault and appsettings.json for connection strings
- **Stored Procedure Focused**: Optimized for stored procedure execution with comprehensive parameter handling
- **Service Layer**: High-level service abstractions for common database operations
- **Attribute-Based Configuration**: Declarative configuration using custom attributes
- **Multi-Result Set Support**: Handles stored procedures returning multiple tables
- **Pagination Support**: Built-in pagination capabilities with filtering
- **Transaction Management**: Unit of Work pattern implementation
- **Performance Optimized**: Uses Dapper for high-performance data access

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Data" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- Dapper 2.1.66
- Microsoft.Data.SqlClient 6.0.2
- Microsoft.Extensions.* (Configuration, DI, Caching, Logging)
- Newtonsoft.Json 13.0.3
- Nexus.Framework.Common
- Nexus.Framework.Vault

## Configuration

### Basic Configuration

Add to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=true;",
    "ReadConnection": "Server=readonly-server;Database=MyApp;Trusted_Connection=true;",
    "WriteConnection": "Server=write-server;Database=MyApp;Trusted_Connection=true;"
  },
  "HybridVaultConnection": {
    "UseVaultCredentials": false,
    "VaultEndpoint": "https://vault.company.com",
    "VaultSecretPath": "database/myapp",
    "FallbackToConnectionString": true,
    "CacheExpirationMinutes": 30
  }
}
```

### Service Registration

Register the services in your `Program.cs`:

```csharp
using Nexus.Framework.Data;

var builder = WebApplication.CreateBuilder(args);

// Basic registration
builder.Services.AddNexusData(builder.Configuration);

// Or with connection string (for compatibility)
builder.Services.AddNexusDataWithConnectionString("your-connection-string");

// Or with CQRS and Vault configuration
builder.Services.AddNexusDataWithCQRS(builder.Configuration, enableVault: true);

var app = builder.Build();
```

### Vault Configuration

For HashiCorp Vault integration:

```json
{
  "HybridVaultConnection": {
    "UseVaultCredentials": true,
    "VaultEndpoint": "https://vault.company.com",
    "VaultSecretPath": "database/myapp",
    "FallbackToConnectionString": true,
    "CacheExpirationMinutes": 30
  }
}
```

## Architecture

### Core Interfaces

#### IRepositoryBase

The foundation repository interface for database operations:

```csharp
public interface IRepositoryBase
{
    Task<DbQueryResult<T>> ExecuteQueryAsync<T>(IDbModel model);
    Task<DbExecutionResult> ExecuteStoredProcedureAsync(IDbModel model);
    Task<MultipleResultSets> ExecuteMultipleQueriesAsync(IDbModel model);
    Task<PagedResult<T>> ExecutePaginatedQueryAsync<T>(IDbModel model, int pageNumber, int pageSize);
    // ... other methods
}
```

#### IServiceBase

High-level service interface with specialized methods:

```csharp
public interface IServiceBase
{
    // Pagination
    Task<PagedResult<T>> ExecutePaginatedQueryAsync<T>(IDbModel model, int pageNumber, int pageSize);
    
    // List operations
    Task<List<T>> RetrieveCompleteListAsync<T>(IDbModel model);
    Task<List<TResult>> ExecuteCustomListQueryAsync<TInput, TResult>(TInput model) where TInput : IDbModel;
    Task<List<T>> ExecuteDynamicListQueryAsync<T>(IDbModel model);
    
    // Single record operations
    Task<TResult> RetrieveSingleCustomResultAsync<TInput, TResult>(TInput model) where TInput : IDbModel;
    Task<T> FindByNumericIdAsync<T>(IDbModel model);
    Task<T> FindByLongIdAsync<T>(IDbModel model);
    
    // Business operations
    Task<DbExecutionResult> ExecuteBusinessOperationAsync<T>(IDbModel model);
    
    // Multi-table operations
    Task<(List<T1>, List<T2>)> ExecuteMultiTableQueryAsync<T1, T2>(IDbModel model);
    Task<(List<T1>, List<T2>, List<T3>)> ExecuteTripleTableQueryAsync<T1, T2, T3>(IDbModel model);
}
```

#### IDbModel

Core interface for database models:

```csharp
public interface IDbModel
{
    string GetSqlCommand();
    CommandType GetCommandType();
    object GetParameters();
}
```

### CQRS Implementation

The framework automatically routes operations based on stored procedure naming conventions:

#### Read Operations (routed to Read connection)
- `LST_` - List operations
- `SNG_` - Single record operations
- `RPT_` - Report operations
- `FNC_` - Function operations

#### Write Operations (routed to Write connection)
- `APP_` - Application/Business logic operations
- `INS_` - Insert operations
- `UPD_` - Update operations
- `DEL_` - Delete operations

#### Connection Types
```csharp
public enum ConnectionType
{
    Default,
    Read,
    Write
}
```

## Usage Examples

### 1. Basic Model Definition

```csharp
[NexusCommand("LST_TB_USUARIOS")]
public class UsuariosInputModel : BaseInputModel
{
    [NexusParameter("@NomeUsuario", SqlDbType.VarChar, 100)]
    public string NomeUsuario { get; set; }
    
    [NexusParameter("@Ativo", SqlDbType.Bit)]
    public bool? Ativo { get; set; }
    
    [NexusParameter("@DataCriacao", SqlDbType.DateTime)]
    public DateTime? DataCriacao { get; set; }
}

public class UsuarioOutputModel
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}
```

### 2. Repository Usage

```csharp
public class UsuarioRepository
{
    private readonly IRepositoryBase _repository;
    
    public UsuarioRepository(IRepositoryBase repository)
    {
        _repository = repository;
    }
    
    public async Task<List<UsuarioOutputModel>> ListarUsuariosAsync(UsuariosInputModel input)
    {
        var result = await _repository.ExecuteQueryAsync<UsuarioOutputModel>(input);
        return result.Data;
    }
    
    public async Task<DbExecutionResult> CriarUsuarioAsync(CriarUsuarioInputModel input)
    {
        return await _repository.ExecuteStoredProcedureAsync(input);
    }
}
```

### 3. Service Usage

```csharp
public class UsuarioService
{
    private readonly IServiceBase _service;
    
    public UsuarioService(IServiceBase service)
    {
        _service = service;
    }
    
    public async Task<List<UsuarioOutputModel>> ListarTodosUsuariosAsync()
    {
        var input = new UsuariosInputModel();
        return await _service.RetrieveCompleteListAsync<UsuarioOutputModel>(input);
    }
    
    public async Task<PagedResult<UsuarioOutputModel>> ListarUsuariosPaginadoAsync(
        UsuariosInputModel input, int pageNumber, int pageSize)
    {
        return await _service.ExecutePaginatedQueryAsync<UsuarioOutputModel>(
            input, pageNumber, pageSize);
    }
    
    public async Task<UsuarioOutputModel> BuscarUsuarioPorIdAsync(int id)
    {
        var input = new BuscarUsuarioInputModel { Id = id };
        return await _service.FindByNumericIdAsync<UsuarioOutputModel>(input);
    }
}
```

### 4. Pagination Example

```csharp
[NexusCommand("LST_TB_PRODUTOS")]
public class ProdutosInputModel : PagedFilterInputModel
{
    [NexusParameter("@Categoria", SqlDbType.VarChar, 50)]
    public string Categoria { get; set; }
    
    [NexusParameter("@PrecoMinimo", SqlDbType.Decimal)]
    public decimal? PrecoMinimo { get; set; }
    
    [NexusParameter("@PrecoMaximo", SqlDbType.Decimal)]
    public decimal? PrecoMaximo { get; set; }
}

public class ProdutoService
{
    private readonly IServiceBase _service;
    
    public ProdutoService(IServiceBase service)
    {
        _service = service;
    }
    
    public async Task<PagedResult<ProdutoOutputModel>> ListarProdutosPaginadoAsync(
        ProdutosInputModel input)
    {
        return await _service.ExecutePaginatedQueryAsync<ProdutoOutputModel>(
            input, input.PageNumber, input.PageSize);
    }
}
```

### 5. Multiple Result Sets

```csharp
[NexusCommand("RPT_DASHBOARD_VENDAS")]
public class DashboardVendasInputModel : BaseInputModel
{
    [NexusParameter("@MesAno", SqlDbType.VarChar, 7)]
    public string MesAno { get; set; }
}

public class DashboardService
{
    private readonly IServiceBase _service;
    
    public DashboardService(IServiceBase service)
    {
        _service = service;
    }
    
    public async Task<(List<VendaOutputModel>, List<ProdutoOutputModel>)> 
        ObterDashboardVendasAsync(DashboardVendasInputModel input)
    {
        return await _service.ExecuteMultiTableQueryAsync<VendaOutputModel, ProdutoOutputModel>(input);
    }
}
```

### 6. Transaction Management

```csharp
public class PedidoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepositoryBase _repository;
    
    public PedidoService(IUnitOfWork unitOfWork, IRepositoryBase repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }
    
    public async Task<bool> CriarPedidoComItensAsync(CriarPedidoInputModel pedido, 
        List<CriarItemPedidoInputModel> itens)
    {
        using var transaction = _unitOfWork.BeginTransaction();
        
        try
        {
            // Criar pedido
            var resultPedido = await _repository.ExecuteStoredProcedureAsync(pedido);
            if (resultPedido.ReturnCode != 0)
                return false;
            
            // Criar itens do pedido
            foreach (var item in itens)
            {
                var resultItem = await _repository.ExecuteStoredProcedureAsync(item);
                if (resultItem.ReturnCode != 0)
                    return false;
            }
            
            _unitOfWork.Commit();
            return true;
        }
        catch
        {
            // Transaction automatically rolled back on dispose
            return false;
        }
    }
}
```

### 7. Custom Connection String Provider

```csharp
public class CustomConnectionStringProvider : IConnectionStringProvider
{
    public async Task<string> GetConnectionStringAsync(ConnectionType connectionType)
    {
        return connectionType switch
        {
            ConnectionType.Read => "ReadConnectionString",
            ConnectionType.Write => "WriteConnectionString",
            _ => "DefaultConnectionString"
        };
    }
}

// Register in DI
builder.Services.AddScoped<IConnectionStringProvider, CustomConnectionStringProvider>();
```

## Advanced Features

### Attribute Configuration

#### NexusCommandAttribute
Decorates classes with stored procedure names:

```csharp
[NexusCommand("LST_TB_USUARIOS")]
public class UsuariosInputModel : BaseInputModel
{
    // Properties
}
```

#### NexusParameterAttribute
Configures parameter mapping:

```csharp
[NexusParameter("@NomeUsuario", SqlDbType.VarChar, 100, ParameterDirection.Input)]
public string NomeUsuario { get; set; }

[NexusParameter("@TotalRegistros", SqlDbType.Int, ParameterDirection.Output)]
public int TotalRegistros { get; set; }
```

### Input Model Hierarchy

#### BaseInputModel
Abstract base class for all input models:

```csharp
public abstract class BaseInputModel : IDbModel
{
    public abstract string GetSqlCommand();
    public virtual CommandType GetCommandType() => CommandType.StoredProcedure;
    public virtual object GetParameters() => this;
}
```

#### PagedFilterInputModel
Specialized for paginated queries:

```csharp
public class PagedFilterInputModel : BaseInputModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortField { get; set; }
    public string SortDirection { get; set; } = "ASC";
}
```

### Result Types

#### DbExecutionResult
Standard result for stored procedure execution:

```csharp
public class DbExecutionResult
{
    public int ReturnCode { get; set; }
    public Dictionary<string, object> OutputParameters { get; set; }
    public bool IsSuccess => ReturnCode == 0;
}
```

#### DbQueryResult<T>
Result for queries returning data:

```csharp
public class DbQueryResult<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int ReturnCode { get; set; }
}
```

#### PagedResult<T>
Standardized pagination result:

```csharp
public class PagedResult<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
```

## Best Practices

### 1. Model Design

```csharp
// ✅ Good: Clear naming convention
[NexusCommand("LST_TB_USUARIOS")]
public class ListarUsuariosInputModel : BaseInputModel
{
    [NexusParameter("@NomeUsuario", SqlDbType.VarChar, 100)]
    public string NomeUsuario { get; set; }
}

// ❌ Bad: Unclear naming
[NexusCommand("GetUsers")]
public class UserInput : BaseInputModel
{
    public string Name { get; set; }
}
```

### 2. Repository Implementation

```csharp
// ✅ Good: Separate concerns
public class UsuarioRepository
{
    private readonly IRepositoryBase _repository;
    
    public UsuarioRepository(IRepositoryBase repository)
    {
        _repository = repository;
    }
    
    public async Task<List<UsuarioOutputModel>> ListarAsync(ListarUsuariosInputModel input)
    {
        var result = await _repository.ExecuteQueryAsync<UsuarioOutputModel>(input);
        return result.Data;
    }
}

// ❌ Bad: Mixed concerns
public class UsuarioRepository
{
    public async Task<List<UsuarioOutputModel>> ListarAsync(string nome)
    {
        // Direct SQL or mixed logic
    }
}
```

### 3. Service Layer

```csharp
// ✅ Good: High-level abstractions
public class UsuarioService
{
    private readonly IServiceBase _service;
    
    public async Task<PagedResult<UsuarioOutputModel>> ListarPaginadoAsync(
        ListarUsuariosInputModel input)
    {
        return await _service.ExecutePaginatedQueryAsync<UsuarioOutputModel>(
            input, input.PageNumber, input.PageSize);
    }
}
```

### 4. Error Handling

```csharp
public class UsuarioService
{
    public async Task<DbExecutionResult> CriarUsuarioAsync(CriarUsuarioInputModel input)
    {
        try
        {
            var result = await _service.ExecuteBusinessOperationAsync<UsuarioOutputModel>(input);
            
            if (!result.IsSuccess)
            {
                // Log error details
                _logger.LogError("Erro ao criar usuário: {ReturnCode}", result.ReturnCode);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar usuário");
            throw;
        }
    }
}
```

## Performance Considerations

### 1. Connection Pooling
The framework uses connection pooling automatically through the underlying ADO.NET provider.

### 2. Caching
Connection strings are cached when using HybridConnectionStringProvider:

```json
{
  "HybridVaultConnection": {
    "CacheExpirationMinutes": 30
  }
}
```

### 3. Parameter Optimization
Use strongly-typed parameters with appropriate sizes:

```csharp
[NexusParameter("@NomeUsuario", SqlDbType.VarChar, 100)]
public string NomeUsuario { get; set; }
```

### 4. Pagination
Always use pagination for large datasets:

```csharp
public async Task<PagedResult<T>> GetDataAsync(int pageNumber, int pageSize)
{
    return await _service.ExecutePaginatedQueryAsync<T>(input, pageNumber, pageSize);
}
```

## Troubleshooting

### Common Issues

1. **Connection String Not Found**
   - Verify configuration in appsettings.json
   - Check Vault connectivity if using Vault integration

2. **Stored Procedure Not Found**
   - Verify stored procedure name in NexusCommand attribute
   - Ensure stored procedure exists in database

3. **Parameter Mapping Issues**
   - Check parameter names match stored procedure parameters
   - Verify SqlDbType matches database parameter type

4. **CQRS Routing Issues**
   - Verify stored procedure naming convention
   - Check connection string configuration for read/write connections

### Logging

Enable detailed logging in appsettings.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Nexus.Framework.Data": "Debug"
    }
  }
}
```

## Migration Guide

### From Direct ADO.NET

1. Create input models inheriting from BaseInputModel
2. Add NexusCommand and NexusParameter attributes
3. Replace direct ADO.NET code with repository/service calls
4. Update dependency injection configuration

### From Entity Framework

1. Replace DbContext with IRepositoryBase/IServiceBase
2. Convert LINQ queries to stored procedures
3. Update model classes to use framework conventions
4. Adjust service layer to use framework patterns

## Contributing

When contributing to this framework:

1. Follow the established naming conventions
2. Add comprehensive unit tests
3. Update documentation
4. Ensure backward compatibility
5. Follow the attribute-based configuration pattern

## Version History

- **1.1.0**: Current version with CQRS support and Vault integration
- **1.0.0**: Initial release with basic repository pattern

## Support

For issues and questions:
- Check the troubleshooting section
- Review the sample implementation in the Sample project
- Consult the framework documentation
- Contact the development team