# Nexus.Framework.File

## Overview

Nexus.Framework.File is a comprehensive file processing framework designed to handle CSV and Excel file operations with enterprise-grade features. Built with type safety, performance, and extensibility in mind, it provides intuitive APIs for reading, writing, and manipulating structured data files. The framework supports automatic type conversion, custom mapping, culture-aware parsing, and comprehensive error handling.

## Key Features

- **CSV Processing**: Full CSV read/write support with automatic delimiter detection
- **Excel Processing**: Advanced Excel manipulation using ClosedXML
- **Type Safety**: Generic type support with automatic property mapping
- **Custom Mapping**: Support for custom row mappers and column mapping
- **Culture Support**: Brazilian and invariant culture parsing for numbers and dates
- **Error Resilience**: Graceful handling of malformed data with continued processing
- **Encoding Support**: Configurable encoding options (UTF-8 default)
- **Automatic Type Conversion**: Support for primitives, DateTime, enums, and nullable types
- **Comprehensive Logging**: Structured logging throughout all operations
- **Memory Efficient**: Stream-based processing where possible

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.File" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- ClosedXML 0.105.0 (for Excel operations)
- Microsoft.Extensions.Logging.Abstractions 9.0.6
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.6

## Configuration

### Service Registration

Register file processing services in your `Program.cs`:

```csharp
using Nexus.Framework.File;

var builder = WebApplication.CreateBuilder(args);

// Register all file services (CSV + Excel)
builder.Services.AddFrameworkFileHandling();

// Or register specific services
builder.Services.AddFrameworkCsvHandling();
builder.Services.AddFrameworkExcelHandling();

var app = builder.Build();
```

## Core Interfaces

### ICsvService

Interface for CSV file operations:

```csharp
public interface ICsvService
{
    Task<List<T>> ReadCsvFileAsync<T>(string filePath, Encoding? encoding = null, string? delimiter = null, bool hasHeaders = true) where T : class, new();
    Task<List<T>> ReadCsvContentAsync<T>(string csvContent, string? delimiter = null, bool hasHeaders = true) where T : class, new();
    Task CreateCsvFileAsync<T>(List<T> data, string filePath, Encoding? encoding = null, string? delimiter = null, bool includeHeaders = true) where T : class;
    Task<string> ConvertToCsvStringAsync<T>(List<T> data, string? delimiter = null, bool includeHeaders = true) where T : class;
    List<string> SplitCsvLine(string line, string delimiter = ",");
}
```

### IExcelService

Interface for Excel file operations:

```csharp
public interface IExcelService
{
    Task<List<T>> ReadExcelSheetAsync<T>(string filePath, string? worksheetName = null, Func<IXLRow, T>? customRowMapper = null) where T : class, new();
    Dictionary<string, int> CreateColumnMap(IXLRow headerRow);
    string GetCellStringValue(IXLCell cell);
    decimal GetCellDecimalValue(IXLCell cell);
    double GetCellDoubleValue(IXLCell cell);
    DateTime? GetCellDateValue(IXLCell cell);
    Task CreateExcelFileAsync<T>(List<T> data, string filePath, string? worksheetName = null) where T : class;
}
```

## Usage Examples

### 1. Basic CSV Operations

```csharp
public class ProductImportService
{
    private readonly ICsvService _csvService;
    private readonly ILogger<ProductImportService> _logger;
    
    public ProductImportService(ICsvService csvService, ILogger<ProductImportService> logger)
    {
        _csvService = csvService;
        _logger = logger;
    }
    
    public async Task<List<Product>> ImportProductsFromCsvAsync(string filePath)
    {
        try
        {
            var products = await _csvService.ReadCsvFileAsync<Product>(filePath, hasHeaders: true);
            _logger.LogInformation("Successfully imported {Count} products", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing products from CSV file: {FilePath}", filePath);
            throw;
        }
    }
    
    public async Task ExportProductsToCsvAsync(List<Product> products, string filePath)
    {
        await _csvService.CreateCsvFileAsync(products, filePath, includeHeaders: true);
        _logger.LogInformation("Exported {Count} products to CSV file: {FilePath}", products.Count, filePath);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public ProductCategory Category { get; set; }
}

public enum ProductCategory
{
    Electronics,
    Clothing,
    Books,
    Home
}
```

### 2. Advanced CSV Processing

```csharp
public class SalesReportService
{
    private readonly ICsvService _csvService;
    
    public SalesReportService(ICsvService csvService)
    {
        _csvService = csvService;
    }
    
    public async Task<List<SalesRecord>> ProcessSalesDataAsync(string csvContent)
    {
        // Process CSV content with custom delimiter
        var salesData = await _csvService.ReadCsvContentAsync<SalesRecord>(
            csvContent, 
            delimiter: ";", 
            hasHeaders: true);
        
        return salesData.Where(s => s.Amount > 0).ToList();
    }
    
    public async Task<string> GenerateSalesReportAsync(List<SalesRecord> sales)
    {
        // Convert to CSV string with custom formatting
        var csvString = await _csvService.ConvertToCsvStringAsync(
            sales, 
            delimiter: ",", 
            includeHeaders: true);
        
        return csvString;
    }
    
    public async Task ProcessLargeCsvFileAsync(string filePath)
    {
        // Process large CSV file with UTF-8 encoding
        var records = await _csvService.ReadCsvFileAsync<SalesRecord>(
            filePath, 
            encoding: Encoding.UTF8, 
            delimiter: ",", 
            hasHeaders: true);
        
        // Process in batches to avoid memory issues
        const int batchSize = 1000;
        for (int i = 0; i < records.Count; i += batchSize)
        {
            var batch = records.Skip(i).Take(batchSize).ToList();
            await ProcessBatchAsync(batch);
        }
    }
    
    private async Task ProcessBatchAsync(List<SalesRecord> batch)
    {
        // Process batch logic here
        await Task.Delay(100); // Simulate processing
    }
}

public class SalesRecord
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public string Region { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
}
```

### 3. Basic Excel Operations

```csharp
public class EmployeeService
{
    private readonly IExcelService _excelService;
    private readonly ILogger<EmployeeService> _logger;
    
    public EmployeeService(IExcelService excelService, ILogger<EmployeeService> logger)
    {
        _excelService = excelService;
        _logger = logger;
    }
    
    public async Task<List<Employee>> ImportEmployeesFromExcelAsync(string filePath)
    {
        try
        {
            // Read from specific worksheet
            var employees = await _excelService.ReadExcelSheetAsync<Employee>(
                filePath, 
                worksheetName: "Employees");
            
            _logger.LogInformation("Successfully imported {Count} employees from Excel", employees.Count);
            return employees;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing employees from Excel file: {FilePath}", filePath);
            throw;
        }
    }
    
    public async Task ExportEmployeesToExcelAsync(List<Employee> employees, string filePath)
    {
        await _excelService.CreateExcelFileAsync(
            employees, 
            filePath, 
            worksheetName: "Employee Report");
        
        _logger.LogInformation("Exported {Count} employees to Excel file: {FilePath}", employees.Count, filePath);
    }
}

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
```

### 4. Advanced Excel Processing with Custom Mapping

```csharp
public class CustomerService
{
    private readonly IExcelService _excelService;
    
    public CustomerService(IExcelService excelService)
    {
        _excelService = excelService;
    }
    
    public async Task<List<Customer>> ImportCustomersWithCustomMappingAsync(string filePath)
    {
        // Use custom row mapper for complex scenarios
        var customers = await _excelService.ReadExcelSheetAsync<Customer>(
            filePath, 
            worksheetName: "Customers",
            customRowMapper: row => new Customer
            {
                Id = int.Parse(_excelService.GetCellStringValue(row.Cell(1))),
                FullName = _excelService.GetCellStringValue(row.Cell(2)),
                Email = _excelService.GetCellStringValue(row.Cell(3)),
                Phone = _excelService.GetCellStringValue(row.Cell(4)),
                RegistrationDate = _excelService.GetCellDateValue(row.Cell(5)) ?? DateTime.MinValue,
                CreditLimit = _excelService.GetCellDecimalValue(row.Cell(6)),
                IsVip = _excelService.GetCellStringValue(row.Cell(7)).ToLower() == "yes",
                Score = _excelService.GetCellDoubleValue(row.Cell(8))
            });
        
        return customers;
    }
    
    public async Task ProcessComplexExcelFileAsync(string filePath)
    {
        // Read first worksheet to get column mapping
        var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);
        var headerRow = worksheet.Row(1);
        var columnMap = _excelService.CreateColumnMap(headerRow);
        
        // Process data with dynamic column mapping
        var customers = new List<Customer>();
        
        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var customer = new Customer
            {
                Id = int.Parse(_excelService.GetCellStringValue(row.Cell(columnMap["CustomerId"]))),
                FullName = _excelService.GetCellStringValue(row.Cell(columnMap["CustomerName"])),
                Email = _excelService.GetCellStringValue(row.Cell(columnMap["Email"])),
                CreditLimit = _excelService.GetCellDecimalValue(row.Cell(columnMap["CreditLimit"]))
            };
            
            customers.Add(customer);
        }
        
        workbook.Dispose();
        
        // Process customers
        await ProcessCustomersAsync(customers);
    }
    
    private async Task ProcessCustomersAsync(List<Customer> customers)
    {
        // Process customers logic
        await Task.CompletedTask;
    }
}

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsVip { get; set; }
    public double Score { get; set; }
}
```

### 5. Bulk Data Processing

```csharp
public class BulkDataService
{
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    private readonly ILogger<BulkDataService> _logger;
    
    public BulkDataService(ICsvService csvService, IExcelService excelService, ILogger<BulkDataService> logger)
    {
        _csvService = csvService;
        _excelService = excelService;
        _logger = logger;
    }
    
    public async Task ProcessMultipleFilesAsync(List<string> filePaths)
    {
        var tasks = filePaths.Select(ProcessSingleFileAsync);
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessSingleFileAsync(string filePath)
    {
        try
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".csv":
                    await ProcessCsvFileAsync(filePath);
                    break;
                case ".xlsx":
                case ".xls":
                    await ProcessExcelFileAsync(filePath);
                    break;
                default:
                    _logger.LogWarning("Unsupported file type: {Extension}", extension);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file: {FilePath}", filePath);
        }
    }
    
    private async Task ProcessCsvFileAsync(string filePath)
    {
        var data = await _csvService.ReadCsvFileAsync<DataRecord>(filePath);
        _logger.LogInformation("Processed {Count} records from CSV: {FilePath}", data.Count, filePath);
        
        // Process data
        await ProcessDataRecordsAsync(data);
    }
    
    private async Task ProcessExcelFileAsync(string filePath)
    {
        var data = await _excelService.ReadExcelSheetAsync<DataRecord>(filePath);
        _logger.LogInformation("Processed {Count} records from Excel: {FilePath}", data.Count, filePath);
        
        // Process data
        await ProcessDataRecordsAsync(data);
    }
    
    private async Task ProcessDataRecordsAsync(List<DataRecord> records)
    {
        // Bulk processing logic
        const int batchSize = 500;
        
        for (int i = 0; i < records.Count; i += batchSize)
        {
            var batch = records.Skip(i).Take(batchSize).ToList();
            await ProcessBatchAsync(batch);
        }
    }
    
    private async Task ProcessBatchAsync(List<DataRecord> batch)
    {
        // Process batch in parallel
        var tasks = batch.Select(ProcessRecordAsync);
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessRecordAsync(DataRecord record)
    {
        // Individual record processing
        await Task.Delay(10); // Simulate processing
    }
}

public class DataRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime ProcessedDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
```

### 6. File Conversion Service

```csharp
public class FileConversionService
{
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    
    public FileConversionService(ICsvService csvService, IExcelService excelService)
    {
        _csvService = csvService;
        _excelService = excelService;
    }
    
    public async Task ConvertCsvToExcelAsync<T>(string csvFilePath, string excelFilePath, string worksheetName = "Data") where T : class, new()
    {
        // Read CSV data
        var data = await _csvService.ReadCsvFileAsync<T>(csvFilePath);
        
        // Write to Excel
        await _excelService.CreateExcelFileAsync(data, excelFilePath, worksheetName);
    }
    
    public async Task ConvertExcelToCsvAsync<T>(string excelFilePath, string csvFilePath, string? worksheetName = null) where T : class, new()
    {
        // Read Excel data
        var data = await _excelService.ReadExcelSheetAsync<T>(excelFilePath, worksheetName);
        
        // Write to CSV
        await _csvService.CreateCsvFileAsync(data, csvFilePath);
    }
    
    public async Task ConvertMultipleWorksheetsToCsvAsync<T>(string excelFilePath, string outputDirectory) where T : class, new()
    {
        using var workbook = new XLWorkbook(excelFilePath);
        
        foreach (var worksheet in workbook.Worksheets)
        {
            var data = await _excelService.ReadExcelSheetAsync<T>(excelFilePath, worksheet.Name);
            
            var csvFileName = $"{worksheet.Name}.csv";
            var csvFilePath = Path.Combine(outputDirectory, csvFileName);
            
            await _csvService.CreateCsvFileAsync(data, csvFilePath);
        }
    }
}
```

## Advanced Features

### 1. Custom Type Conversion

```csharp
public class CustomTypeConverter
{
    public static object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GetDefaultValue(targetType);
        
        try
        {
            if (targetType == typeof(string))
                return value;
            
            if (targetType == typeof(int) || targetType == typeof(int?))
                return int.Parse(value);
            
            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                return decimal.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return bool.Parse(value);
            
            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, true);
            
            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch
        {
            return GetDefaultValue(targetType);
        }
    }
    
    private static object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
```

### 2. File Validation

```csharp
public class FileValidationService
{
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    
    public FileValidationService(ICsvService csvService, IExcelService excelService)
    {
        _csvService = csvService;
        _excelService = excelService;
    }
    
    public async Task<ValidationResult> ValidateCsvFileAsync<T>(string filePath, Func<T, ValidationResult> validator) where T : class, new()
    {
        var result = new ValidationResult();
        
        try
        {
            var data = await _csvService.ReadCsvFileAsync<T>(filePath);
            
            for (int i = 0; i < data.Count; i++)
            {
                var itemValidation = validator(data[i]);
                if (!itemValidation.IsValid)
                {
                    result.Errors.Add($"Row {i + 2}: {string.Join(", ", itemValidation.Errors)}");
                }
            }
            
            result.IsValid = !result.Errors.Any();
        }
        catch (Exception ex)
        {
            result.Errors.Add($"File processing error: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }
    
    public async Task<ValidationResult> ValidateExcelFileAsync<T>(string filePath, Func<T, ValidationResult> validator, string? worksheetName = null) where T : class, new()
    {
        var result = new ValidationResult();
        
        try
        {
            var data = await _excelService.ReadExcelSheetAsync<T>(filePath, worksheetName);
            
            for (int i = 0; i < data.Count; i++)
            {
                var itemValidation = validator(data[i]);
                if (!itemValidation.IsValid)
                {
                    result.Errors.Add($"Row {i + 2}: {string.Join(", ", itemValidation.Errors)}");
                }
            }
            
            result.IsValid = !result.Errors.Any();
        }
        catch (Exception ex)
        {
            result.Errors.Add($"File processing error: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
}
```

### 3. Streaming Processing

```csharp
public class StreamingFileProcessor
{
    private readonly ICsvService _csvService;
    private readonly ILogger<StreamingFileProcessor> _logger;
    
    public StreamingFileProcessor(ICsvService csvService, ILogger<StreamingFileProcessor> logger)
    {
        _csvService = csvService;
        _logger = logger;
    }
    
    public async Task ProcessLargeFileAsync<T>(string filePath, Func<T, Task> processor) where T : class, new()
    {
        const int batchSize = 1000;
        int processedCount = 0;
        
        using var reader = new StreamReader(filePath);
        var lines = new List<string>();
        
        // Read header
        var headerLine = await reader.ReadLineAsync();
        if (headerLine == null) return;
        
        while (!reader.EndOfStream)
        {
            lines.Clear();
            
            // Read batch
            for (int i = 0; i < batchSize && !reader.EndOfStream; i++)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                    lines.Add(line);
            }
            
            if (lines.Count == 0) break;
            
            // Process batch
            var csvContent = headerLine + "\n" + string.Join("\n", lines);
            var data = await _csvService.ReadCsvContentAsync<T>(csvContent);
            
            foreach (var item in data)
            {
                await processor(item);
                processedCount++;
            }
            
            _logger.LogInformation("Processed {Count} records so far", processedCount);
        }
        
        _logger.LogInformation("Completed processing {TotalCount} records", processedCount);
    }
}
```

## Best Practices

### 1. Error Handling

```csharp
public class SafeFileProcessor
{
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    private readonly ILogger<SafeFileProcessor> _logger;
    
    public SafeFileProcessor(ICsvService csvService, IExcelService excelService, ILogger<SafeFileProcessor> logger)
    {
        _csvService = csvService;
        _excelService = excelService;
        _logger = logger;
    }
    
    public async Task<List<T>> SafeReadFileAsync<T>(string filePath) where T : class, new()
    {
        try
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                _logger.LogError("File not found: {FilePath}", filePath);
                return new List<T>();
            }
            
            // Check file size
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > 100 * 1024 * 1024) // 100MB limit
            {
                _logger.LogWarning("File size exceeds 100MB: {FilePath}", filePath);
            }
            
            // Determine file type and process
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            return extension switch
            {
                ".csv" => await _csvService.ReadCsvFileAsync<T>(filePath),
                ".xlsx" or ".xls" => await _excelService.ReadExcelSheetAsync<T>(filePath),
                _ => throw new NotSupportedException($"Unsupported file type: {extension}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            throw;
        }
    }
}
```

### 2. Performance Optimization

```csharp
public class OptimizedFileProcessor
{
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    
    public OptimizedFileProcessor(ICsvService csvService, IExcelService excelService)
    {
        _csvService = csvService;
        _excelService = excelService;
    }
    
    public async Task<List<T>> ProcessFileAsync<T>(string filePath, int maxRecords = 10000) where T : class, new()
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            List<T> data;
            
            switch (extension)
            {
                case ".csv":
                    data = await _csvService.ReadCsvFileAsync<T>(filePath);
                    break;
                case ".xlsx":
                case ".xls":
                    data = await _excelService.ReadExcelSheetAsync<T>(filePath);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported file type: {extension}");
            }
            
            // Limit records for performance
            if (data.Count > maxRecords)
            {
                data = data.Take(maxRecords).ToList();
            }
            
            return data;
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine($"File processing took {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
```

### 3. Configuration Management

```csharp
public class FileProcessingOptions
{
    public string DefaultEncoding { get; set; } = "UTF-8";
    public string DefaultDelimiter { get; set; } = ",";
    public int MaxFileSize { get; set; } = 100 * 1024 * 1024; // 100MB
    public int BatchSize { get; set; } = 1000;
    public bool LogProcessingDetails { get; set; } = true;
    public string TempDirectory { get; set; } = Path.GetTempPath();
}

public class ConfigurableFileService
{
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    private readonly FileProcessingOptions _options;
    
    public ConfigurableFileService(ICsvService csvService, IExcelService excelService, IOptions<FileProcessingOptions> options)
    {
        _csvService = csvService;
        _excelService = excelService;
        _options = options.Value;
    }
    
    public async Task<List<T>> ReadFileAsync<T>(string filePath) where T : class, new()
    {
        var encoding = Encoding.GetEncoding(_options.DefaultEncoding);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        return extension switch
        {
            ".csv" => await _csvService.ReadCsvFileAsync<T>(filePath, encoding, _options.DefaultDelimiter),
            ".xlsx" or ".xls" => await _excelService.ReadExcelSheetAsync<T>(filePath),
            _ => throw new NotSupportedException($"Unsupported file type: {extension}")
        };
    }
}
```

## Testing

### 1. Unit Testing

```csharp
[Test]
public async Task ReadCsvFileAsync_WithValidData_ShouldReturnCorrectObjects()
{
    // Arrange
    var csvContent = "Id,Name,Price\n1,Product1,10.50\n2,Product2,20.00";
    var tempFile = Path.GetTempFileName();
    await File.WriteAllTextAsync(tempFile, csvContent);
    
    var csvService = new CsvService(Mock.Of<ILogger<CsvService>>());
    
    // Act
    var result = await csvService.ReadCsvFileAsync<Product>(tempFile);
    
    // Assert
    Assert.AreEqual(2, result.Count);
    Assert.AreEqual("Product1", result[0].Name);
    Assert.AreEqual(10.50m, result[0].Price);
    
    // Cleanup
    File.Delete(tempFile);
}

[Test]
public async Task ReadExcelSheetAsync_WithValidData_ShouldReturnCorrectObjects()
{
    // Arrange
    var tempFile = Path.GetTempFileName() + ".xlsx";
    var testData = new List<Product>
    {
        new Product { Id = 1, Name = "Product1", Price = 10.50m },
        new Product { Id = 2, Name = "Product2", Price = 20.00m }
    };
    
    var excelService = new ExcelService(Mock.Of<ILogger<ExcelService>>());
    await excelService.CreateExcelFileAsync(testData, tempFile);
    
    // Act
    var result = await excelService.ReadExcelSheetAsync<Product>(tempFile);
    
    // Assert
    Assert.AreEqual(2, result.Count);
    Assert.AreEqual("Product1", result[0].Name);
    Assert.AreEqual(10.50m, result[0].Price);
    
    // Cleanup
    File.Delete(tempFile);
}
```

### 2. Integration Testing

```csharp
[Test]
public async Task FileProcessing_EndToEnd_ShouldWorkCorrectly()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddFrameworkFileHandling();
    
    var provider = services.BuildServiceProvider();
    var csvService = provider.GetRequiredService<ICsvService>();
    var excelService = provider.GetRequiredService<IExcelService>();
    
    var testData = new List<Product>
    {
        new Product { Id = 1, Name = "Test Product", Price = 99.99m, CreatedDate = DateTime.Now, IsActive = true }
    };
    
    var csvFile = Path.GetTempFileName() + ".csv";
    var excelFile = Path.GetTempFileName() + ".xlsx";
    
    try
    {
        // Act - Create files
        await csvService.CreateCsvFileAsync(testData, csvFile);
        await excelService.CreateExcelFileAsync(testData, excelFile);
        
        // Act - Read files
        var csvResult = await csvService.ReadCsvFileAsync<Product>(csvFile);
        var excelResult = await excelService.ReadExcelSheetAsync<Product>(excelFile);
        
        // Assert
        Assert.AreEqual(1, csvResult.Count);
        Assert.AreEqual(1, excelResult.Count);
        Assert.AreEqual(testData[0].Name, csvResult[0].Name);
        Assert.AreEqual(testData[0].Name, excelResult[0].Name);
    }
    finally
    {
        // Cleanup
        if (File.Exists(csvFile)) File.Delete(csvFile);
        if (File.Exists(excelFile)) File.Delete(excelFile);
    }
}
```

## Performance Considerations

### 1. Memory Management

```csharp
public class MemoryEfficientProcessor
{
    public async Task ProcessLargeFileAsync<T>(string filePath, Func<T, Task> processor) where T : class, new()
    {
        const int batchSize = 500;
        
        using var reader = new StreamReader(filePath);
        var buffer = new List<string>(batchSize);
        
        // Read header
        var header = await reader.ReadLineAsync();
        
        while (!reader.EndOfStream)
        {
            buffer.Clear();
            
            // Fill buffer
            for (int i = 0; i < batchSize && !reader.EndOfStream; i++)
            {
                var line = await reader.ReadLineAsync();
                if (line != null) buffer.Add(line);
            }
            
            // Process buffer
            if (buffer.Count > 0)
            {
                var csvContent = header + "\n" + string.Join("\n", buffer);
                // Process batch without holding all data in memory
                await ProcessBatchAsync(csvContent, processor);
            }
        }
    }
    
    private async Task ProcessBatchAsync<T>(string csvContent, Func<T, Task> processor) where T : class, new()
    {
        // Process batch logic
        await Task.CompletedTask;
    }
}
```

### 2. Parallel Processing

```csharp
public class ParallelFileProcessor
{
    public async Task ProcessFilesInParallelAsync<T>(List<string> filePaths, Func<T, Task> processor) where T : class, new()
    {
        var semaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
        
        var tasks = filePaths.Select(async filePath =>
        {
            await semaphore.WaitAsync();
            try
            {
                await ProcessSingleFileAsync(filePath, processor);
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessSingleFileAsync<T>(string filePath, Func<T, Task> processor) where T : class, new()
    {
        // Process single file
        await Task.CompletedTask;
    }
}
```

## Troubleshooting

### Common Issues

1. **File Encoding Problems**
   - Specify correct encoding when reading files
   - Use UTF-8 with BOM for Excel compatibility

2. **Memory Issues with Large Files**
   - Implement streaming processing
   - Process files in batches
   - Use appropriate batch sizes

3. **Type Conversion Errors**
   - Implement custom type converters
   - Validate data before processing
   - Use nullable types for optional fields

4. **Excel Format Issues**
   - Ensure proper column mapping
   - Handle missing worksheets gracefully
   - Use appropriate data types

### Debugging

```csharp
// Enable detailed logging
{
  "Logging": {
    "LogLevel": {
      "Nexus.Framework.File": "Debug"
    }
  }
}
```

## Migration Guide

### From Custom CSV Processing

```csharp
// Before
var lines = File.ReadAllLines("file.csv");
var data = new List<Product>();
foreach (var line in lines.Skip(1))
{
    var values = line.Split(',');
    data.Add(new Product
    {
        Id = int.Parse(values[0]),
        Name = values[1],
        Price = decimal.Parse(values[2])
    });
}

// After
var data = await _csvService.ReadCsvFileAsync<Product>("file.csv");
```

### From Direct Excel Processing

```csharp
// Before
using var workbook = new XLWorkbook("file.xlsx");
var worksheet = workbook.Worksheet(1);
var data = new List<Product>();
foreach (var row in worksheet.RowsUsed().Skip(1))
{
    data.Add(new Product
    {
        Id = int.Parse(row.Cell(1).Value.ToString()),
        Name = row.Cell(2).Value.ToString(),
        Price = decimal.Parse(row.Cell(3).Value.ToString())
    });
}

// After
var data = await _excelService.ReadExcelSheetAsync<Product>("file.xlsx");
```

## Version History

- **1.1.0**: Current version with CSV and Excel support
- **1.0.0**: Initial release with basic file processing

## Support

For issues and questions:
- Check the troubleshooting section
- Review the usage examples
- Consult the framework documentation
- Contact the development team