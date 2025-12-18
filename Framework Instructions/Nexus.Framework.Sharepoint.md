# Nexus.Framework.Sharepoint

## Overview

Nexus.Framework.Sharepoint is a comprehensive SharePoint integration framework built on Microsoft Graph API. It provides a simplified interface for common SharePoint operations including file upload, download, deletion, and listing. The framework handles authentication, site resolution, and error management while offering a clean, async-first API that integrates seamlessly with modern .NET applications.

## Key Features

- **File Operations**: Upload, download, delete, and list files in SharePoint document libraries
- **Microsoft Graph Integration**: Built on the official Microsoft Graph SDK
- **Azure AD Authentication**: OAuth 2.0 Client Credentials flow with proper token management
- **Async/Await Support**: Full asynchronous operations with cancellation token support
- **Error Handling**: Comprehensive error handling with detailed logging
- **Path Normalization**: Robust path handling for cross-platform compatibility
- **Overwrite Protection**: Configurable file overwrite behavior
- **Structured Logging**: Integrated logging for monitoring and debugging
- **Dependency Injection**: Full DI container integration

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Sharepoint" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- Microsoft.Graph 5.83.0
- Microsoft.Identity.Client 4.73.1
- Microsoft.Extensions.Configuration 9.0.6
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.6
- Microsoft.Extensions.Logging.Abstractions 9.0.6

## Configuration

### Azure AD App Registration

1. Register an application in Azure AD
2. Grant the following permissions:
   - `Sites.Read.All`
   - `Sites.ReadWrite.All` 
   - `Files.Read.All`
   - `Files.ReadWrite.All`
3. Create a client secret
4. Note the Tenant ID, Client ID, and Client Secret

### Application Configuration

Add SharePoint settings to your `appsettings.json`:

```json
{
  "SharePoint": {
    "Graph": {
      "TenantId": "your-tenant-id",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret"
    }
  }
}
```

### Service Registration

Register SharePoint services in your `Program.cs`:

```csharp
using Nexus.Framework.Sharepoint;

var builder = WebApplication.CreateBuilder(args);

// Register SharePoint services
builder.Services.AddFrameworkSharePoint(builder.Configuration);

// Or register Graph SharePoint service directly
builder.Services.AddFrameworkGraphSharePoint();

var app = builder.Build();
```

## Core Interface

### ISharePointService

The main interface for SharePoint operations:

```csharp
public interface ISharePointService
{
    Task<string> UploadFileAsync(string siteUrl, string folderPath, string fileName, byte[] fileContent, bool overwrite = false, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<List<SharePointFile>> ListFilesAsync(string siteUrl, string folderPath, CancellationToken cancellationToken = default);
}
```

### SharePointFile Model

Represents SharePoint file metadata:

```csharp
public class SharePointFile
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}
```

## Usage Examples

### 1. Basic File Upload

```csharp
public class DocumentService
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<DocumentService> _logger;
    
    public DocumentService(ISharePointService sharePointService, ILogger<DocumentService> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    public async Task<string> UploadDocumentAsync(string siteUrl, string fileName, byte[] fileContent)
    {
        try
        {
            var fileUrl = await _sharePointService.UploadFileAsync(
                siteUrl: siteUrl,
                folderPath: "Documents",
                fileName: fileName,
                fileContent: fileContent,
                overwrite: false
            );
            
            _logger.LogInformation("Document uploaded successfully: {FileUrl}", fileUrl);
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload document: {FileName}", fileName);
            throw;
        }
    }
    
    public async Task<string> UploadToCustomFolderAsync(string siteUrl, string folderPath, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        
        var fileContent = memoryStream.ToArray();
        
        return await _sharePointService.UploadFileAsync(
            siteUrl: siteUrl,
            folderPath: folderPath,
            fileName: file.FileName,
            fileContent: fileContent,
            overwrite: true
        );
    }
}
```

### 2. File Download Operations

```csharp
public class FileDownloadService
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<FileDownloadService> _logger;
    
    public FileDownloadService(ISharePointService sharePointService, ILogger<FileDownloadService> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    public async Task<byte[]> DownloadFileAsync(string fileUrl)
    {
        try
        {
            var fileContent = await _sharePointService.DownloadFileAsync(fileUrl);
            
            _logger.LogInformation("File downloaded successfully: {FileUrl}", fileUrl);
            return fileContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file: {FileUrl}", fileUrl);
            throw;
        }
    }
    
    public async Task<IActionResult> DownloadFileToResponseAsync(string fileUrl, string fileName)
    {
        try
        {
            var fileContent = await _sharePointService.DownloadFileAsync(fileUrl);
            
            var contentType = GetContentType(fileName);
            
            return new FileContentResult(fileContent, contentType)
            {
                FileDownloadName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file for response: {FileUrl}", fileUrl);
            return new NotFoundResult();
        }
    }
    
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }
}
```

### 3. File Management Operations

```csharp
public class FileManagementService
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<FileManagementService> _logger;
    
    public FileManagementService(ISharePointService sharePointService, ILogger<FileManagementService> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    public async Task<List<SharePointFile>> GetFilesAsync(string siteUrl, string folderPath)
    {
        try
        {
            var files = await _sharePointService.ListFilesAsync(siteUrl, folderPath);
            
            _logger.LogInformation("Retrieved {Count} files from folder: {FolderPath}", files.Count, folderPath);
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve files from folder: {FolderPath}", folderPath);
            throw;
        }
    }
    
    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var success = await _sharePointService.DeleteFileAsync(fileUrl);
            
            if (success)
            {
                _logger.LogInformation("File deleted successfully: {FileUrl}", fileUrl);
            }
            else
            {
                _logger.LogWarning("Failed to delete file: {FileUrl}", fileUrl);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            throw;
        }
    }
    
    public async Task<List<SharePointFile>> SearchFilesAsync(string siteUrl, string folderPath, string searchTerm)
    {
        var allFiles = await _sharePointService.ListFilesAsync(siteUrl, folderPath);
        
        var filteredFiles = allFiles.Where(f => 
            f.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        _logger.LogInformation("Found {Count} files matching search term: {SearchTerm}", filteredFiles.Count, searchTerm);
        return filteredFiles;
    }
}
```

### 4. Bulk Operations

```csharp
public class BulkFileService
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<BulkFileService> _logger;
    
    public BulkFileService(ISharePointService sharePointService, ILogger<BulkFileService> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    public async Task<List<string>> UploadMultipleFilesAsync(string siteUrl, string folderPath, List<FileUploadModel> files)
    {
        var uploadedUrls = new List<string>();
        var semaphore = new SemaphoreSlim(5, 5); // Limit concurrent uploads
        
        var tasks = files.Select(async file =>
        {
            await semaphore.WaitAsync();
            try
            {
                var fileUrl = await _sharePointService.UploadFileAsync(
                    siteUrl: siteUrl,
                    folderPath: folderPath,
                    fileName: file.FileName,
                    fileContent: file.Content,
                    overwrite: file.Overwrite
                );
                
                lock (uploadedUrls)
                {
                    uploadedUrls.Add(fileUrl);
                }
                
                _logger.LogInformation("Uploaded file: {FileName}", file.FileName);
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file: {FileName}", file.FileName);
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
        
        _logger.LogInformation("Bulk upload completed: {SuccessCount}/{TotalCount}", 
            uploadedUrls.Count, files.Count);
        
        return uploadedUrls;
    }
    
    public async Task<int> DeleteMultipleFilesAsync(List<string> fileUrls)
    {
        var deleteCount = 0;
        var semaphore = new SemaphoreSlim(5, 5);
        
        var tasks = fileUrls.Select(async fileUrl =>
        {
            await semaphore.WaitAsync();
            try
            {
                var success = await _sharePointService.DeleteFileAsync(fileUrl);
                if (success)
                {
                    Interlocked.Increment(ref deleteCount);
                }
                return success;
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
        
        _logger.LogInformation("Bulk delete completed: {DeleteCount}/{TotalCount}", 
            deleteCount, fileUrls.Count);
        
        return deleteCount;
    }
}

public class FileUploadModel
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public bool Overwrite { get; set; }
}
```

### 5. File Synchronization Service

```csharp
public class FileSynchronizationService
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<FileSynchronizationService> _logger;
    
    public FileSynchronizationService(ISharePointService sharePointService, ILogger<FileSynchronizationService> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    public async Task SynchronizeFilesAsync(string sourceSiteUrl, string targetSiteUrl, string folderPath)
    {
        try
        {
            // Get files from source
            var sourceFiles = await _sharePointService.ListFilesAsync(sourceSiteUrl, folderPath);
            
            // Get files from target
            var targetFiles = await _sharePointService.ListFilesAsync(targetSiteUrl, folderPath);
            
            // Find files that need to be synchronized
            var filesToSync = sourceFiles.Where(sourceFile => 
                !targetFiles.Any(targetFile => 
                    targetFile.Name == sourceFile.Name && 
                    targetFile.Modified >= sourceFile.Modified))
                .ToList();
            
            _logger.LogInformation("Found {Count} files to synchronize", filesToSync.Count);
            
            // Synchronize files
            foreach (var file in filesToSync)
            {
                try
                {
                    var fileContent = await _sharePointService.DownloadFileAsync(file.Url);
                    
                    await _sharePointService.UploadFileAsync(
                        siteUrl: targetSiteUrl,
                        folderPath: folderPath,
                        fileName: file.Name,
                        fileContent: fileContent,
                        overwrite: true
                    );
                    
                    _logger.LogInformation("Synchronized file: {FileName}", file.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to synchronize file: {FileName}", file.Name);
                }
            }
            
            _logger.LogInformation("File synchronization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file synchronization");
            throw;
        }
    }
}
```

### 6. Web API Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class SharePointController : ControllerBase
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<SharePointController> _logger;
    
    public SharePointController(ISharePointService sharePointService, ILogger<SharePointController> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string siteUrl, [FromQuery] string folderPath)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided");
        }
        
        try
        {
            using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            
            var fileUrl = await _sharePointService.UploadFileAsync(
                siteUrl: siteUrl,
                folderPath: folderPath,
                fileName: file.FileName,
                fileContent: memoryStream.ToArray(),
                overwrite: false
            );
            
            return Ok(new { FileUrl = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", file.FileName);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpGet("download")]
    public async Task<IActionResult> DownloadFile([FromQuery] string fileUrl)
    {
        try
        {
            var fileContent = await _sharePointService.DownloadFileAsync(fileUrl);
            
            var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
            var contentType = "application/octet-stream";
            
            return File(fileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file: {FileUrl}", fileUrl);
            return NotFound();
        }
    }
    
    [HttpGet("files")]
    public async Task<IActionResult> ListFiles([FromQuery] string siteUrl, [FromQuery] string folderPath)
    {
        try
        {
            var files = await _sharePointService.ListFilesAsync(siteUrl, folderPath);
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list files in folder: {FolderPath}", folderPath);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFile([FromQuery] string fileUrl)
    {
        try
        {
            var success = await _sharePointService.DeleteFileAsync(fileUrl);
            
            if (success)
            {
                return Ok(new { Message = "File deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "File not found" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FileUrl}", fileUrl);
            return StatusCode(500, "Internal server error");
        }
    }
}
```

## Advanced Features

### 1. Custom Authentication Provider

```csharp
public class CustomAuthenticationProvider : IAuthenticationProvider
{
    private readonly IConfidentialClientApplication _app;
    private readonly string[] _scopes;
    
    public CustomAuthenticationProvider(IConfidentialClientApplication app, string[] scopes)
    {
        _app = app;
        _scopes = scopes;
    }
    
    public async Task AuthenticateRequestAsync(HttpRequestMessage request)
    {
        var result = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
    }
}
```

### 2. File Validation Service

```csharp
public class FileValidationService
{
    private readonly ILogger<FileValidationService> _logger;
    private readonly List<string> _allowedExtensions = new() { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".png" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    
    public FileValidationService(ILogger<FileValidationService> logger)
    {
        _logger = logger;
    }
    
    public ValidationResult ValidateFile(string fileName, byte[] fileContent)
    {
        var result = new ValidationResult();
        
        // Check file extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            result.Errors.Add($"File extension '{extension}' is not allowed");
        }
        
        // Check file size
        if (fileContent.Length > MaxFileSize)
        {
            result.Errors.Add($"File size {fileContent.Length} exceeds maximum allowed size of {MaxFileSize}");
        }
        
        // Check file content (basic validation)
        if (fileContent.Length == 0)
        {
            result.Errors.Add("File content is empty");
        }
        
        result.IsValid = !result.Errors.Any();
        
        if (!result.IsValid)
        {
            _logger.LogWarning("File validation failed for {FileName}: {Errors}", 
                fileName, string.Join(", ", result.Errors));
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

### 3. Retry Policy Service

```csharp
public class RetryPolicyService
{
    private readonly ILogger<RetryPolicyService> _logger;
    
    public RetryPolicyService(ILogger<RetryPolicyService> logger)
    {
        _logger = logger;
    }
    
    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (attempt < maxRetries && IsRetryableException(ex))
            {
                _logger.LogWarning("Operation failed on attempt {Attempt}: {Message}. Retrying...", 
                    attempt, ex.Message);
                
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
            }
        }
        
        // Final attempt without catching exceptions
        return await operation();
    }
    
    private bool IsRetryableException(Exception ex)
    {
        return ex is HttpRequestException ||
               ex is TaskCanceledException ||
               (ex is Microsoft.Graph.ServiceException graphEx && 
                graphEx.Error.Code == "TooManyRequests");
    }
}
```

### 4. Configuration Validation

```csharp
public class SharePointConfigurationValidator
{
    public static void ValidateConfiguration(IConfiguration configuration)
    {
        var sharePointConfig = configuration.GetSection("SharePoint:Graph");
        
        var tenantId = sharePointConfig["TenantId"];
        var clientId = sharePointConfig["ClientId"];
        var clientSecret = sharePointConfig["ClientSecret"];
        
        if (string.IsNullOrEmpty(tenantId))
            throw new InvalidOperationException("SharePoint:Graph:TenantId is required");
        
        if (string.IsNullOrEmpty(clientId))
            throw new InvalidOperationException("SharePoint:Graph:ClientId is required");
        
        if (string.IsNullOrEmpty(clientSecret))
            throw new InvalidOperationException("SharePoint:Graph:ClientSecret is required");
        
        // Validate GUID format
        if (!Guid.TryParse(tenantId, out _))
            throw new InvalidOperationException("SharePoint:Graph:TenantId must be a valid GUID");
        
        if (!Guid.TryParse(clientId, out _))
            throw new InvalidOperationException("SharePoint:Graph:ClientId must be a valid GUID");
    }
}
```

## Best Practices

### 1. Error Handling

```csharp
public class SharePointServiceWrapper
{
    private readonly ISharePointService _sharePointService;
    private readonly ILogger<SharePointServiceWrapper> _logger;
    
    public SharePointServiceWrapper(ISharePointService sharePointService, ILogger<SharePointServiceWrapper> logger)
    {
        _sharePointService = sharePointService;
        _logger = logger;
    }
    
    public async Task<string> SafeUploadFileAsync(string siteUrl, string folderPath, string fileName, byte[] fileContent)
    {
        try
        {
            return await _sharePointService.UploadFileAsync(siteUrl, folderPath, fileName, fileContent);
        }
        catch (Microsoft.Graph.ServiceException ex) when (ex.Error.Code == "Forbidden")
        {
            _logger.LogError("Access denied when uploading file: {FileName}", fileName);
            throw new UnauthorizedAccessException("Insufficient permissions to upload file");
        }
        catch (Microsoft.Graph.ServiceException ex) when (ex.Error.Code == "NotFound")
        {
            _logger.LogError("Site or folder not found: {SiteUrl}/{FolderPath}", siteUrl, folderPath);
            throw new DirectoryNotFoundException("Site or folder not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error uploading file: {FileName}", fileName);
            throw;
        }
    }
}
```

### 2. Performance Optimization

```csharp
public class OptimizedSharePointService
{
    private readonly ISharePointService _sharePointService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<OptimizedSharePointService> _logger;
    
    public OptimizedSharePointService(
        ISharePointService sharePointService, 
        IMemoryCache cache, 
        ILogger<OptimizedSharePointService> logger)
    {
        _sharePointService = sharePointService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<List<SharePointFile>> GetCachedFilesAsync(string siteUrl, string folderPath)
    {
        var cacheKey = $"files_{siteUrl}_{folderPath}";
        
        if (_cache.TryGetValue(cacheKey, out List<SharePointFile>? cachedFiles))
        {
            _logger.LogInformation("Retrieved files from cache for {SiteUrl}/{FolderPath}", siteUrl, folderPath);
            return cachedFiles!;
        }
        
        var files = await _sharePointService.ListFilesAsync(siteUrl, folderPath);
        
        _cache.Set(cacheKey, files, TimeSpan.FromMinutes(5));
        
        _logger.LogInformation("Cached files for {SiteUrl}/{FolderPath}", siteUrl, folderPath);
        return files;
    }
}
```

### 3. URL Validation

```csharp
public class SharePointUrlValidator
{
    public static bool IsValidSharePointUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
        
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            return false;
        
        var host = uri.Host.ToLowerInvariant();
        
        // Check for SharePoint Online
        if (host.EndsWith(".sharepoint.com") || host.EndsWith(".sharepoint.us"))
            return true;
        
        // Check for on-premises SharePoint (customize as needed)
        if (host.Contains("sharepoint") || uri.LocalPath.Contains("_layouts"))
            return true;
        
        return false;
    }
    
    public static string NormalizeSiteUrl(string siteUrl)
    {
        if (string.IsNullOrWhiteSpace(siteUrl))
            throw new ArgumentException("Site URL cannot be empty");
        
        if (!IsValidSharePointUrl(siteUrl))
            throw new ArgumentException("Invalid SharePoint URL");
        
        return siteUrl.TrimEnd('/');
    }
}
```

## Testing

### 1. Unit Testing

```csharp
[Test]
public async Task UploadFileAsync_WithValidData_ShouldReturnFileUrl()
{
    // Arrange
    var mockSharePointService = new Mock<ISharePointService>();
    var expectedFileUrl = "https://company.sharepoint.com/sites/test/Documents/test.pdf";
    
    mockSharePointService
        .Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFileUrl);
    
    var service = new DocumentService(mockSharePointService.Object, Mock.Of<ILogger<DocumentService>>());
    
    // Act
    var result = await service.UploadDocumentAsync(
        "https://company.sharepoint.com/sites/test",
        "test.pdf",
        new byte[] { 1, 2, 3 }
    );
    
    // Assert
    Assert.AreEqual(expectedFileUrl, result);
    mockSharePointService.Verify(s => s.UploadFileAsync(
        It.IsAny<string>(),
        "Documents",
        "test.pdf",
        It.IsAny<byte[]>(),
        false,
        It.IsAny<CancellationToken>()), Times.Once);
}
```

### 2. Integration Testing

```csharp
[Test]
public async Task SharePointIntegration_ShouldWorkEndToEnd()
{
    // Arrange
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["SharePoint:Graph:TenantId"] = "test-tenant-id",
            ["SharePoint:Graph:ClientId"] = "test-client-id",
            ["SharePoint:Graph:ClientSecret"] = "test-client-secret"
        })
        .Build();
    
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(configuration);
    services.AddLogging();
    services.AddFrameworkSharePoint(configuration);
    
    var provider = services.BuildServiceProvider();
    var sharePointService = provider.GetRequiredService<ISharePointService>();
    
    // Act & Assert
    Assert.IsNotNull(sharePointService);
    Assert.IsInstanceOf<GraphSharePointService>(sharePointService);
}
```

## Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify Azure AD app registration
   - Check client secret expiration
   - Ensure proper API permissions

2. **File Upload Failures**
   - Validate file size limits
   - Check folder permissions
   - Verify site URL format

3. **Site Resolution Issues**
   - Ensure site URL is accessible
   - Check site collection permissions
   - Verify Graph API permissions

4. **Network Timeouts**
   - Implement retry policies
   - Check network connectivity
   - Consider file size limitations

### Debugging

```csharp
// Enable detailed logging
{
  "Logging": {
    "LogLevel": {
      "Nexus.Framework.Sharepoint": "Debug",
      "Microsoft.Graph": "Debug"
    }
  }
}
```

## Performance Considerations

### 1. File Size Limits

```csharp
public class FileSizeValidator
{
    private const long MaxFileSize = 250 * 1024 * 1024; // 250MB SharePoint limit
    
    public static bool IsValidFileSize(byte[] fileContent)
    {
        return fileContent.Length <= MaxFileSize;
    }
}
```

### 2. Concurrent Operations

```csharp
public class ConcurrentSharePointService
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10);
    
    public async Task<string> UploadWithConcurrencyControlAsync(
        string siteUrl, 
        string folderPath, 
        string fileName, 
        byte[] fileContent)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _sharePointService.UploadFileAsync(siteUrl, folderPath, fileName, fileContent);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

## Migration Guide

### From SharePoint CSOM

```csharp
// Before (CSOM)
var context = new ClientContext(siteUrl);
var web = context.Web;
var list = web.Lists.GetByTitle("Documents");
var file = list.RootFolder.Files.Add(fileInfo);

// After (Framework)
var fileUrl = await _sharePointService.UploadFileAsync(siteUrl, "Documents", fileName, fileContent);
```

### From Direct Graph API

```csharp
// Before (Direct Graph)
var graphClient = new GraphServiceClient(authProvider);
var site = await graphClient.Sites.GetByPath(sitePath, hostname).Request().GetAsync();
var driveItem = await graphClient.Sites[site.Id].Drive.Root.ItemWithPath(filePath).Content.Request().PutAsync(stream);

// After (Framework)
var fileUrl = await _sharePointService.UploadFileAsync(siteUrl, folderPath, fileName, fileContent);
```

## Version History

- **1.1.0**: Current version with Microsoft Graph integration
- **1.0.0**: Initial release with basic SharePoint functionality

## Support

For issues and questions:
- Check the troubleshooting section
- Review the Azure AD app registration
- Consult the framework documentation
- Contact the development team