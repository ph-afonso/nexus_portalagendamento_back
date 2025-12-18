# Nexus.Framework.Email

## Overview

Nexus.Framework.Email is a comprehensive email service framework that provides a unified interface for sending emails through multiple providers. The framework supports both traditional SMTP and modern Microsoft Graph API email sending, with seamless provider switching through configuration. Built with enterprise requirements in mind, it offers robust error handling, attachment support, and extensive logging capabilities.

## Key Features

- **Unified Interface**: Single `IEmailService` interface for all email providers
- **Multiple Providers**: Support for SMTP and Microsoft Graph API
- **Provider Switching**: Easy configuration-based provider selection
- **Attachment Support**: Full support for file attachments
- **HTML/Plain Text**: Support for both HTML and plain text emails
- **Multiple Recipients**: Support for To, CC, and BCC recipients
- **Async Operations**: Fully asynchronous email sending
- **Comprehensive Logging**: Detailed logging for debugging and monitoring
- **Error Resilience**: Robust error handling and recovery
- **OAuth2 Integration**: Microsoft Graph authentication support

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Nexus.Framework.Email" Version="1.1.0" />
```

## Dependencies

This framework depends on:
- .NET 9.0
- MailKit 4.13.0 (for SMTP functionality)
- Microsoft.Graph 5.83.0 (for Graph API integration)
- Microsoft.Identity.Client 4.73.1 (for OAuth authentication)
- Microsoft.Extensions.Configuration 9.0.6
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions

## Configuration

### Basic Configuration

Add email settings to your `appsettings.json`:

#### SMTP Configuration

```json
{
  "Email": {
    "Provider": "smtp",
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Username": "your-email@gmail.com",
      "Password": "your-password",
      "EnableSsl": "true",
      "From": "noreply@company.com",
      "FromName": "Company System"
    }
  }
}
```

#### Microsoft Graph Configuration

```json
{
  "Email": {
    "Provider": "graph",
    "Graph": {
      "TenantId": "your-tenant-id",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret"
    }
  }
}
```

### Service Registration

Register email services in your `Program.cs`:

```csharp
using Nexus.Framework.Email;

var builder = WebApplication.CreateBuilder(args);

// Auto-detect provider based on configuration
builder.Services.AddFrameworkEmail(builder.Configuration);

// Or explicitly specify provider
builder.Services.AddFrameworkSmtpEmail();
builder.Services.AddFrameworkGraphEmail();

var app = builder.Build();
```

## Core Interface

### IEmailService

The unified interface for all email operations:

```csharp
public interface IEmailService
{
    Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        List<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default);
}
```

### EmailAttachment

Class for handling email attachments:

```csharp
public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "application/octet-stream";
}
```

## Usage Examples

### 1. Basic Email Sending

```csharp
public class NotificationService
{
    private readonly IEmailService _emailService;
    
    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task SendWelcomeEmailAsync(string userEmail, string userName)
    {
        var subject = "Welcome to Our Platform";
        var body = $"<h1>Welcome {userName}!</h1><p>Thank you for joining our platform.</p>";
        
        var success = await _emailService.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: true
        );
        
        if (!success)
        {
            // Handle email sending failure
            throw new InvalidOperationException("Failed to send welcome email");
        }
    }
}
```

### 2. Email with Multiple Recipients

```csharp
public class ReportService
{
    private readonly IEmailService _emailService;
    
    public ReportService(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task SendMonthlyReportAsync(MonthlyReport report)
    {
        var subject = $"Monthly Report - {report.Month:MMMM yyyy}";
        var body = GenerateReportEmailBody(report);
        
        var toRecipients = new List<string> { "manager@company.com" };
        var ccRecipients = new List<string> { "team@company.com", "director@company.com" };
        var bccRecipients = new List<string> { "archive@company.com" };
        
        var success = await _emailService.SendEmailAsync(
            to: string.Join(",", toRecipients),
            subject: subject,
            body: body,
            isHtml: true,
            cc: ccRecipients,
            bcc: bccRecipients
        );
        
        if (success)
        {
            _logger.LogInformation("Monthly report sent successfully");
        }
    }
}
```

### 3. Email with Attachments

```csharp
public class DocumentService
{
    private readonly IEmailService _emailService;
    private readonly IFileService _fileService;
    
    public DocumentService(IEmailService emailService, IFileService fileService)
    {
        _emailService = emailService;
        _fileService = fileService;
    }
    
    public async Task SendDocumentAsync(string userEmail, string documentPath)
    {
        // Read document content
        var documentContent = await _fileService.ReadFileAsync(documentPath);
        var fileName = Path.GetFileName(documentPath);
        
        // Create attachment
        var attachment = new EmailAttachment
        {
            FileName = fileName,
            Content = documentContent,
            ContentType = GetContentType(fileName)
        };
        
        var subject = $"Document: {fileName}";
        var body = $"<p>Please find the attached document: <strong>{fileName}</strong></p>";
        
        var success = await _emailService.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: true,
            attachments: new List<EmailAttachment> { attachment }
        );
        
        if (success)
        {
            _logger.LogInformation("Document sent successfully to {Email}", userEmail);
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

### 4. Email Templates

```csharp
public class EmailTemplateService
{
    private readonly IEmailService _emailService;
    
    public EmailTemplateService(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task<bool> SendPasswordResetEmailAsync(string userEmail, string resetToken)
    {
        var subject = "Password Reset Request";
        var body = GetPasswordResetTemplate(resetToken);
        
        return await _emailService.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: true
        );
    }
    
    public async Task<bool> SendOrderConfirmationEmailAsync(string userEmail, Order order)
    {
        var subject = $"Order Confirmation - #{order.OrderNumber}";
        var body = GetOrderConfirmationTemplate(order);
        
        return await _emailService.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: true
        );
    }
    
    private string GetPasswordResetTemplate(string resetToken)
    {
        return $@"
            <html>
            <body>
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Please click the link below to reset your password:</p>
                <p><a href='https://yourapp.com/reset-password?token={resetToken}'>Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
            </body>
            </html>";
    }
    
    private string GetOrderConfirmationTemplate(Order order)
    {
        var itemsHtml = string.Join("", order.Items.Select(item => 
            $"<tr><td>{item.Name}</td><td>{item.Quantity}</td><td>${item.Price:F2}</td></tr>"));
        
        return $@"
            <html>
            <body>
                <h2>Order Confirmation</h2>
                <p>Thank you for your order! Here are the details:</p>
                
                <h3>Order #{order.OrderNumber}</h3>
                <table border='1' style='border-collapse: collapse;'>
                    <tr><th>Item</th><th>Quantity</th><th>Price</th></tr>
                    {itemsHtml}
                </table>
                
                <p><strong>Total: ${order.Total:F2}</strong></p>
                <p>Expected delivery: {order.ExpectedDelivery:MM/dd/yyyy}</p>
            </body>
            </html>";
    }
}
```

### 5. Bulk Email Service

```csharp
public class BulkEmailService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BulkEmailService> _logger;
    
    public BulkEmailService(IEmailService emailService, ILogger<BulkEmailService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task<int> SendBulkEmailAsync(List<string> recipients, string subject, string body)
    {
        var successCount = 0;
        var semaphore = new SemaphoreSlim(10, 10); // Limit concurrent sends
        
        var tasks = recipients.Select(async email =>
        {
            await semaphore.WaitAsync();
            try
            {
                var success = await _emailService.SendEmailAsync(
                    to: email,
                    subject: subject,
                    body: body,
                    isHtml: true
                );
                
                if (success)
                {
                    Interlocked.Increment(ref successCount);
                }
                else
                {
                    _logger.LogWarning("Failed to send email to {Email}", email);
                }
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
        
        _logger.LogInformation("Bulk email completed: {SuccessCount}/{TotalCount}", 
            successCount, recipients.Count);
        
        return successCount;
    }
}
```

### 6. Email Queue Service

```csharp
public class EmailQueueService : BackgroundService
{
    private readonly IEmailService _emailService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailQueueService> _logger;
    private readonly Channel<EmailQueueItem> _queue;
    
    public EmailQueueService(
        IEmailService emailService,
        IServiceProvider serviceProvider,
        ILogger<EmailQueueService> logger)
    {
        _emailService = emailService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _queue = Channel.CreateUnbounded<EmailQueueItem>();
    }
    
    public async Task QueueEmailAsync(EmailQueueItem item)
    {
        await _queue.Writer.WriteAsync(item);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                var success = await _emailService.SendEmailAsync(
                    to: item.To,
                    subject: item.Subject,
                    body: item.Body,
                    isHtml: item.IsHtml,
                    cc: item.CC,
                    bcc: item.BCC,
                    attachments: item.Attachments,
                    cancellationToken: stoppingToken
                );
                
                if (success)
                {
                    _logger.LogInformation("Email sent successfully to {To}", item.To);
                }
                else
                {
                    _logger.LogError("Failed to send email to {To}", item.To);
                    
                    // Retry logic could be added here
                    if (item.RetryCount < 3)
                    {
                        item.RetryCount++;
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        await QueueEmailAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email queue item");
            }
        }
    }
}

public class EmailQueueItem
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
    public List<string>? CC { get; set; }
    public List<string>? BCC { get; set; }
    public List<EmailAttachment>? Attachments { get; set; }
    public int RetryCount { get; set; }
    public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
}
```

## Provider Implementations

### SMTP Provider

The SMTP provider uses MailKit for robust email sending:

```csharp
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;
    
    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        List<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new SmtpClient();
            
            // Configure SMTP client
            await client.ConnectAsync(
                _configuration["Email:Smtp:Host"],
                int.Parse(_configuration["Email:Smtp:Port"]),
                bool.Parse(_configuration["Email:Smtp:EnableSsl"]),
                cancellationToken);
            
            await client.AuthenticateAsync(
                _configuration["Email:Smtp:Username"],
                _configuration["Email:Smtp:Password"],
                cancellationToken);
            
            // Create message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:Smtp:FromName"],
                _configuration["Email:Smtp:From"]));
            
            // Add recipients
            foreach (var recipient in to.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(MailboxAddress.Parse(recipient.Trim()));
            }
            
            // Add CC recipients
            if (cc != null)
            {
                foreach (var ccRecipient in cc)
                {
                    message.Cc.Add(MailboxAddress.Parse(ccRecipient));
                }
            }
            
            // Add BCC recipients
            if (bcc != null)
            {
                foreach (var bccRecipient in bcc)
                {
                    message.Bcc.Add(MailboxAddress.Parse(bccRecipient));
                }
            }
            
            message.Subject = subject;
            
            // Create body
            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            
            // Add attachments
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(
                        attachment.FileName,
                        attachment.Content,
                        ContentType.Parse(attachment.ContentType));
                }
            }
            
            message.Body = bodyBuilder.ToMessageBody();
            
            // Send message
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
            
            _logger.LogInformation("Email sent successfully to {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }
}
```

### Microsoft Graph Provider

The Graph provider uses Microsoft Graph API for email sending:

```csharp
public class GraphEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GraphEmailService> _logger;
    
    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        List<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create Graph client
            var app = ConfidentialClientApplicationBuilder
                .Create(_configuration["Email:Graph:ClientId"])
                .WithClientSecret(_configuration["Email:Graph:ClientSecret"])
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{_configuration["Email:Graph:TenantId"]}"))
                .Build();
            
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    var result = await app.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
                        .ExecuteAsync(cancellationToken);
                    requestMessage.Headers.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
                }));
            
            // Create message
            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = isHtml ? BodyType.Html : BodyType.Text,
                    Content = body
                },
                ToRecipients = to.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(email => new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = email.Trim()
                        }
                    }).ToList()
            };
            
            // Add CC recipients
            if (cc != null && cc.Any())
            {
                message.CcRecipients = cc.Select(email => new Recipient
                {
                    EmailAddress = new EmailAddress { Address = email }
                }).ToList();
            }
            
            // Add BCC recipients
            if (bcc != null && bcc.Any())
            {
                message.BccRecipients = bcc.Select(email => new Recipient
                {
                    EmailAddress = new EmailAddress { Address = email }
                }).ToList();
            }
            
            // Add attachments
            if (attachments != null && attachments.Any())
            {
                message.Attachments = attachments.Select(attachment => new FileAttachment
                {
                    Name = attachment.FileName,
                    ContentBytes = attachment.Content,
                    ContentType = attachment.ContentType
                }).ToList<Attachment>();
            }
            
            // Send message
            await graphClient.Me.SendMail(new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            }).PostAsync(cancellationToken);
            
            _logger.LogInformation("Email sent successfully via Graph API to {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via Graph API to {To}", to);
            return false;
        }
    }
}
```

## Advanced Configuration

### Environment-Specific Settings

```csharp
public class EmailConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    
    public EmailConfigurationService(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }
    
    public EmailSettings GetEmailSettings()
    {
        var settings = new EmailSettings();
        
        if (_environment.IsDevelopment())
        {
            // Use file-based email in development
            settings.Provider = "file";
            settings.OutputPath = Path.Combine(Directory.GetCurrentDirectory(), "emails");
        }
        else
        {
            // Use configured provider in production
            settings.Provider = _configuration["Email:Provider"];
        }
        
        return settings;
    }
}
```

### Custom Email Provider

```csharp
public class FileEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileEmailService> _logger;
    
    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        List<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var outputPath = _configuration["Email:File:OutputPath"] ?? "emails";
            Directory.CreateDirectory(outputPath);
            
            var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.eml";
            var filePath = Path.Combine(outputPath, fileName);
            
            var emailContent = $@"
From: {_configuration["Email:From"]}
To: {to}
CC: {string.Join(", ", cc ?? new List<string>())}
BCC: {string.Join(", ", bcc ?? new List<string>())}
Subject: {subject}
Content-Type: {(isHtml ? "text/html" : "text/plain")}; charset=utf-8

{body}

Attachments: {attachments?.Count ?? 0}
";
            
            await File.WriteAllTextAsync(filePath, emailContent, cancellationToken);
            
            _logger.LogInformation("Email saved to file: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save email to file");
            return false;
        }
    }
}

// Register custom provider
builder.Services.AddScoped<IEmailService, FileEmailService>();
```

## Best Practices

### 1. Error Handling

```csharp
public class SafeEmailService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SafeEmailService> _logger;
    
    public async Task<bool> SendEmailWithRetryAsync(
        string to,
        string subject,
        string body,
        int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var success = await _emailService.SendEmailAsync(to, subject, body);
                if (success)
                {
                    return true;
                }
                
                _logger.LogWarning("Email send attempt {Attempt} failed", attempt);
                
                if (attempt < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email send attempt {Attempt} threw exception", attempt);
                
                if (attempt == maxRetries)
                {
                    throw;
                }
            }
        }
        
        return false;
    }
}
```

### 2. Email Validation

```csharp
public class EmailValidator
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);
    
    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }
    
    public static List<string> ValidateEmails(List<string> emails)
    {
        return emails.Where(IsValidEmail).ToList();
    }
}
```

### 3. Configuration Validation

```csharp
public class EmailConfigurationValidator
{
    public static void ValidateConfiguration(IConfiguration configuration)
    {
        var provider = configuration["Email:Provider"];
        
        if (provider == "smtp")
        {
            ValidateSmtpConfiguration(configuration);
        }
        else if (provider == "graph")
        {
            ValidateGraphConfiguration(configuration);
        }
        else
        {
            throw new InvalidOperationException($"Unknown email provider: {provider}");
        }
    }
    
    private static void ValidateSmtpConfiguration(IConfiguration configuration)
    {
        var requiredKeys = new[]
        {
            "Email:Smtp:Host",
            "Email:Smtp:Port",
            "Email:Smtp:Username",
            "Email:Smtp:Password",
            "Email:Smtp:From"
        };
        
        foreach (var key in requiredKeys)
        {
            if (string.IsNullOrEmpty(configuration[key]))
            {
                throw new InvalidOperationException($"Missing required SMTP configuration: {key}");
            }
        }
    }
    
    private static void ValidateGraphConfiguration(IConfiguration configuration)
    {
        var requiredKeys = new[]
        {
            "Email:Graph:TenantId",
            "Email:Graph:ClientId",
            "Email:Graph:ClientSecret"
        };
        
        foreach (var key in requiredKeys)
        {
            if (string.IsNullOrEmpty(configuration[key]))
            {
                throw new InvalidOperationException($"Missing required Graph configuration: {key}");
            }
        }
    }
}
```

## Testing

### 1. Unit Testing

```csharp
[Test]
public async Task SendEmailAsync_WithValidData_ShouldReturnTrue()
{
    // Arrange
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Email:Smtp:Host"] = "smtp.example.com",
            ["Email:Smtp:Port"] = "587",
            ["Email:Smtp:Username"] = "test@example.com",
            ["Email:Smtp:Password"] = "password",
            ["Email:Smtp:EnableSsl"] = "true",
            ["Email:Smtp:From"] = "noreply@example.com",
            ["Email:Smtp:FromName"] = "Test System"
        })
        .Build();
    
    var logger = new Mock<ILogger<SmtpEmailService>>();
    var emailService = new SmtpEmailService(configuration, logger.Object);
    
    // Act
    var result = await emailService.SendEmailAsync(
        "user@example.com",
        "Test Subject",
        "Test Body",
        isHtml: false);
    
    // Assert
    Assert.IsTrue(result);
}
```

### 2. Integration Testing

```csharp
[Test]
public async Task SendEmail_Integration_ShouldSendActualEmail()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddSingleton<IConfiguration>(Configuration);
    services.AddFrameworkEmail(Configuration);
    
    var provider = services.BuildServiceProvider();
    var emailService = provider.GetRequiredService<IEmailService>();
    
    // Act
    var result = await emailService.SendEmailAsync(
        "test@example.com",
        "Integration Test",
        "This is a test email from integration tests",
        isHtml: false);
    
    // Assert
    Assert.IsTrue(result);
}
```

## Performance Considerations

### 1. Attachment Size Limits

```csharp
public class EmailAttachmentValidator
{
    private const int MaxAttachmentSize = 25 * 1024 * 1024; // 25MB
    private const int MaxTotalAttachmentSize = 50 * 1024 * 1024; // 50MB
    
    public static bool ValidateAttachments(List<EmailAttachment> attachments)
    {
        if (attachments == null || !attachments.Any())
            return true;
        
        var totalSize = attachments.Sum(a => a.Content.Length);
        
        if (totalSize > MaxTotalAttachmentSize)
            return false;
        
        return attachments.All(a => a.Content.Length <= MaxAttachmentSize);
    }
}
```

### 2. Connection Pooling

```csharp
public class OptimizedSmtpEmailService : IEmailService
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10);
    
    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        List<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Send email logic here
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

## Troubleshooting

### Common Issues

1. **SMTP Authentication Failures**
   - Check username/password credentials
   - Verify SSL/TLS settings
   - Check firewall/network connectivity

2. **Microsoft Graph Permission Issues**
   - Ensure proper app registration in Azure AD
   - Verify client secret hasn't expired
   - Check API permissions (Mail.Send)

3. **Email Delivery Issues**
   - Check spam/junk folders
   - Verify email addresses are valid
   - Review email provider logs

### Debugging

```csharp
// Enable detailed logging
{
  "Logging": {
    "LogLevel": {
      "Nexus.Framework.Email": "Debug",
      "MailKit": "Debug",
      "Microsoft.Graph": "Debug"
    }
  }
}
```

## Migration Guide

### From System.Net.Mail

```csharp
// Before
var client = new SmtpClient("smtp.example.com", 587);
client.Credentials = new NetworkCredential("user", "password");
client.EnableSsl = true;

var message = new MailMessage("from@example.com", "to@example.com");
message.Subject = "Subject";
message.Body = "Body";

await client.SendMailAsync(message);

// After
await _emailService.SendEmailAsync(
    "to@example.com",
    "Subject",
    "Body");
```

### From Direct Graph API

```csharp
// Before
var graphClient = new GraphServiceClient(authProvider);
var message = new Message { ... };
await graphClient.Me.SendMail(message).Request().PostAsync();

// After
await _emailService.SendEmailAsync(
    "to@example.com",
    "Subject",
    "Body");
```

## Version History

- **1.1.0**: Current version with SMTP and Graph providers
- **1.0.0**: Initial release with basic email functionality

## Support

For issues and questions:
- Check the troubleshooting section
- Review the configuration examples
- Consult the framework documentation
- Contact the development team