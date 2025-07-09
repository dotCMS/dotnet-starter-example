# DotCmsService Documentation

## Overview

The `DotCmsService` is the core service class responsible for interacting with the dotCMS API. It provides methods to fetch pages, execute GraphQL queries, and retrieve navigation data from dotCMS instances. The service implements caching, authentication, and error handling to ensure reliable and performant communication with the dotCMS backend.

## Table of Contents

- [Class Overview](#class-overview)
- [Constructor](#constructor)
- [Configuration](#configuration)
- [Public Methods](#public-methods)
- [Caching Strategy](#caching-strategy)
- [Authentication](#authentication)
- [Error Handling](#error-handling)
- [Security Features](#security-features)
- [Usage Examples](#usage-examples)
- [Performance Considerations](#performance-considerations)

## Class Overview

```csharp
public class DotCmsService : IDotCmsService
```

The `DotCmsService` class implements the `IDotCmsService` interface and provides the following key features:

- **Page API Integration**: Fetch pages using dotCMS Page API
- **GraphQL Support**: Execute GraphQL queries against dotCMS
- **Navigation API**: Retrieve site navigation structure
- **Caching**: Built-in caching with configurable TTL
- **Authentication**: Support for both API tokens and basic authentication
- **Error Handling**: Comprehensive logging and exception handling
- **Security**: Path traversal protection and GraphQL injection prevention

## Constructor

```csharp
public DotCmsService(
    HttpClient httpClient, 
    IConfiguration configuration, 
    ILogger<DotCmsService> logger, 
    IAppCache cache,
    ModelHelper modelHelper)
```

### Parameters

- **`httpClient`**: HTTP client for making API requests
- **`configuration`**: Application configuration containing dotCMS settings
- **`logger`**: Logger instance for diagnostic information
- **`cache`**: LazyCache instance for response caching
- **`modelHelper`**: Helper for converting GraphQL responses to page models

### Exceptions

- **`ArgumentNullException`**: Thrown when any required dependency is null
- **`InvalidOperationException`**: Thrown when required configuration values are missing

## Configuration

The service requires the following configuration values in `appsettings.json`:

```json
{
  "dotCMS": {
    "ApiHost": "https://your-dotcms-instance.com",
    "ApiToken": "your-api-token",
    "ApiUserName": "fallback-username",
    "ApiPassword": "fallback-password",
    "CacheTTL": 120
  }
}
```

### Configuration Keys

| Key | Required | Description | Default |
|-----|----------|-------------|---------|
| `dotCMS:ApiHost` | Yes | dotCMS instance URL | - |
| `dotCMS:ApiToken` | Recommended | API token for authentication | - |
| `dotCMS:ApiUserName` | If no token | Username for basic auth | - |
| `dotCMS:ApiPassword` | If no token | Password for basic auth | - |
| `dotCMS:CacheTTL` | No | Default cache TTL in seconds | 120 |

## Public Methods

### GetPageAsync

Retrieves a page from dotCMS using the Page API.

```csharp
public async Task<PageResponse> GetPageAsync(PageQueryParams queryParams)
```

#### Parameters

- **`queryParams`**: Query parameters containing path, site, mode, language, persona, etc.

#### Returns

- **`PageResponse`**: Complete page data including layout, containers, and contentlets

#### Example

```csharp
var queryParams = new PageQueryParams
{
    Path = "/about-us",
    Site = "demo.dotcms.com",
    PageMode = "LIVE_MODE",
    Language = "1",
    CacheSeconds = 300
};

var pageResponse = await dotCmsService.GetPageAsync(queryParams);
```

### GetPageGraphqlAsync

Retrieves a page from dotCMS using GraphQL.

```csharp
public async Task<PageResponse> GetPageGraphqlAsync(PageQueryParams queryParams)
```

#### Parameters

- **`queryParams`**: Query parameters for the GraphQL request

#### Returns

- **`PageResponse`**: Page data converted from GraphQL response

#### Example

```csharp
var queryParams = new PageQueryParams
{
    Path = "/products",
    Site = "demo.dotcms.com",
    PageMode = "PREVIEW_MODE"
};

var pageResponse = await dotCmsService.GetPageGraphqlAsync(queryParams);
```

### QueryGraphqlAsync

Executes a custom GraphQL query against dotCMS.

```csharp
public async Task<string> QueryGraphqlAsync(string graphqlQuery)
public async Task<string> QueryGraphqlAsync(string graphqlQuery, int cacheSeconds)
```

#### Parameters

- **`graphqlQuery`**: The GraphQL query string
- **`cacheSeconds`**: Optional cache duration (0 = no cache)

#### Returns

- **`string`**: Raw GraphQL response content

#### Example

```csharp
var query = @"
{
  contentlets(contentType: ""Product"", limit: 10) {
    title
    identifier
    _map
  }
}";

var response = await dotCmsService.QueryGraphqlAsync(query, 60);
```

### GetNavigationAsync

Retrieves the site navigation structure.

```csharp
public async Task<NavigationResponse> GetNavigationAsync(int depth = 4)
```

#### Parameters

- **`depth`**: Navigation hierarchy depth (0-10, default: 4)

#### Returns

- **`NavigationResponse`**: Site navigation structure

#### Example

```csharp
var navigation = await dotCmsService.GetNavigationAsync(3);
```

## Caching Strategy

The service implements intelligent caching with different TTL values for different scenarios:

### Cache TTL Values

| Scenario | Default TTL | Configurable |
|----------|-------------|--------------|
| Live Mode Pages | 120 seconds | Yes (dotCMS:CacheTTL) |
| Edit/Preview Mode | 0 seconds | No |
| Navigation | 60 seconds | No |
| GraphQL Queries | 60 seconds | Via parameter |

### Cache Keys

- **Page API**: Full request URL including query parameters
- **GraphQL**: SHA256 hash of the query string
- **Navigation**: Request URL with depth parameter

### Cache Behavior

```csharp
// Live mode - cached
var liveParams = new PageQueryParams { Path = "/home", PageMode = "LIVE_MODE" };
var cachedResponse = await service.GetPageAsync(liveParams); // Cached for 120s

// Edit mode - not cached
var editParams = new PageQueryParams { Path = "/home", PageMode = "EDIT_MODE" };
var freshResponse = await service.GetPageAsync(editParams); // Always fresh

// Custom cache duration
var customParams = new PageQueryParams { 
    Path = "/home", 
    PageMode = "LIVE_MODE",
    CacheSeconds = 300 
};
var customCachedResponse = await service.GetPageAsync(customParams); // Cached for 300s
```

## Authentication

The service supports two authentication methods:

### 1. API Token (Recommended)

```json
{
  "dotCMS": {
    "ApiToken": "your-api-token-here"
  }
}
```

Uses Bearer token authentication:
```
Authorization: Bearer your-api-token-here
```

### 2. Basic Authentication (Fallback)

```json
{
  "dotCMS": {
    "ApiUserName": "admin@dotcms.com",
    "ApiPassword": "admin"
  }
}
```

Uses Basic authentication with base64-encoded credentials:
```
Authorization: Basic YWRtaW5AZG90Y21zLmNvbTphZG1pbg==
```

**Note**: Basic authentication forces a login on every request and is less performant than API tokens.

## Error Handling

The service implements comprehensive error handling:

### Exception Types

- **`ArgumentNullException`**: Invalid method parameters
- **`ArgumentException`**: Invalid path or query parameters
- **`HttpRequestException`**: API communication errors
- **`JsonException`**: Response deserialization errors
- **`InvalidOperationException`**: Configuration errors

### Logging

The service logs important events at different levels:

```csharp
// Information
_logger.LogInformation("Requesting page from: {RequestUrl}", requestUrl);

// Warning
_logger.LogWarning("Navigation depth {Depth} exceeds recommended maximum", depth);

// Error
_logger.LogError(ex, "Error in GetPageAsync for path: {Path}", queryParams.Path);
```

### Error Response Handling

```csharp
private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
{
    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogWarning("API returned status code: {StatusCode}, Content: {Content}", 
            response.StatusCode, errorContent);
        throw new HttpRequestException($"API returned status code: {response.StatusCode}");
    }
}
```

## Security Features

### Path Traversal Protection

The service validates paths to prevent directory traversal attacks:

```csharp
private static string NormalizePath(string? path)
{
    // Security: Prevent path traversal attacks
    if (path.Contains("..") || path.Contains("~") || path.Contains("\\"))
    {
        throw new ArgumentException("Invalid path: Path traversal patterns are not allowed");
    }
    
    // Additional normalization...
}
```

### GraphQL Injection Prevention

GraphQL strings are properly escaped to prevent injection attacks:

```csharp
private static string EscapeGraphqlString(string input)
{
    return input
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\n", "\\n")
        .Replace("\r", "\\r")
        .Replace("\t", "\\t");
}
```

### Input Validation

All public methods validate their parameters:

```csharp
public async Task<PageResponse> GetPageAsync(PageQueryParams queryParams)
{
    ArgumentNullException.ThrowIfNull(queryParams);
    // Method implementation...
}
```

## Usage Examples

### Basic Page Retrieval

```csharp
public class HomeController : Controller
{
    private readonly IDotCmsService _dotCmsService;
    
    public HomeController(IDotCmsService dotCmsService)
    {
        _dotCmsService = dotCmsService;
    }
    
    public async Task<IActionResult> Index()
    {
        var queryParams = new PageQueryParams
        {
            Path = "/",
            PageMode = "LIVE_MODE"
        };
        
        var pageResponse = await _dotCmsService.GetPageAsync(queryParams);
        return View(pageResponse);
    }
}
```

### Multi-language Support

```csharp
public async Task<IActionResult> GetLocalizedPage(string path, string language)
{
    var queryParams = new PageQueryParams
    {
        Path = path,
        Language = language,
        PageMode = "LIVE_MODE",
        CacheSeconds = 300
    };
    
    var pageResponse = await _dotCmsService.GetPageAsync(queryParams);
    return View(pageResponse);
}
```

### Custom GraphQL Query

```csharp
public async Task<IActionResult> GetProducts()
{
    var query = @"
    {
        contentlets(contentType: ""Product"", limit: 20, sortBy: ""modDate desc"") {
            title
            identifier
            modDate
            _map
        }
    }";
    
    var response = await _dotCmsService.QueryGraphqlAsync(query, 120);
    var products = JsonSerializer.Deserialize<ProductResponse>(response);
    
    return View(products);
}
```

### Navigation with Error Handling

```csharp
public async Task<IActionResult> GetNavigation()
{
    try
    {
        var navigation = await _dotCmsService.GetNavigationAsync(3);
        return PartialView("_Navigation", navigation);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Failed to load navigation");
        return PartialView("_NavigationError");
    }
}
```

## Performance Considerations

### Caching Best Practices

1. **Use appropriate cache durations**:
   - Live content: 2-5 minutes
   - Navigation: 1-2 minutes
   - Static content: 10-30 minutes

2. **Disable caching for edit modes**:
   ```csharp
   var editParams = new PageQueryParams 
   { 
       Path = path, 
       PageMode = "EDIT_MODE",
       CacheSeconds = 0  // No caching for edit mode
   };
   ```

3. **Monitor cache hit rates** and adjust TTL values based on content update frequency.

### HTTP Client Configuration

Configure the HTTP client for optimal performance:

```csharp
services.AddHttpClient<DotCmsService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "DotCMS-.NET-SDK/1.0");
});
```

### Memory Management

The service properly disposes of HTTP resources:

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
using var response = await _httpClient.SendAsync(request);
```

## Dependencies

The service requires the following NuGet packages:

- `Microsoft.Extensions.Http` - HTTP client factory
- `Microsoft.Extensions.Configuration` - Configuration management
- `Microsoft.Extensions.Logging` - Logging infrastructure
- `LazyCache` - In-memory caching
- `System.Text.Json` - JSON serialization

## Thread Safety

The `DotCmsService` is thread-safe and can be registered as a singleton in the DI container:

```csharp
services.AddSingleton<IDotCmsService, DotCmsService>();
```

All shared state is immutable, and the LazyCache implementation is thread-safe.
