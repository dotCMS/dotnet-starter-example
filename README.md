![dotcms-logo-square](https://github.com/user-attachments/assets/97d16db7-706c-4f4c-b352-619e1c8ceccc) &nbsp; &nbsp; &nbsp; &nbsp; ![Microsoft_ NET_logo](https://github.com/user-attachments/assets/59b19db7-206e-49c4-83b1-efbaf98ca07f)  &nbsp; 


# dotCMS .NET Example for UVE

This project is a UVE .NET example/base project and is intended to be used as a template for web projects looking to render dotCMS content and pages using [.NET MVC](https://dotnet.microsoft.com/en-us/apps/aspnet/mvc) and [Razor templates](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/).  The example points to our demo site and is pulling the page, content, layout grid and other information from https://demo.dotcms.com

To run the application locally against the demo site with the default settings, use:

```bash
dotnet run
```

The application will be available at `https://localhost:5001`.


## Features

- Render dotCMS content and pages in a .NET application
- Uses C#, .NET MVC and Razor templates for views
- Service-based architecture for API interactions
- Caching support for improved performance
- Proxy support for serving dotCMS assets
- Custom TagHelpers for simplified content rendering
- GraphQL support for advanced querying
- Built-in security features (path traversal protection, GraphQL injection prevention)
- Comprehensive error handling and logging
- Thread-safe service implementation


### Example dotCMS demo site rendering with C#, MVC & Razor

https://github.com/user-attachments/assets/8cdf8952-63bf-4214-af85-f0c1eed3a32c


## How It Works: MVC Pattern & Razor Templates

This application follows the Model-View-Controller (MVC) architectural pattern to render dotCMS content:

### 1. **Controller Layer** (`DotCmsUVEController`)

The `DotCmsUVEController` acts as a catch-all controller that intercepts all incoming requests:

```csharp
[Route("{**catchAll}")]
public async Task<IActionResult> Index(string catchAll, ...)
```

- **Request Handling**: Captures any URL path and forwards it to dotCMS
- **Parameter Processing**: Handles query parameters like `mode`, `language_id`, `personaId`
- **Service Integration**: Uses `IDotCmsService` to fetch page data from dotCMS
- **View Selection**: Returns the appropriate Razor view with the page data model

### 2. **Model Layer** 

The application uses strongly-typed C# models that mirror dotCMS data structures:

- **`PageResponse`**: The main response model containing all page data
- **`Page`**: Metadata about the page (title, tags, permissions)
- **`Layout`**: Defines the page structure (rows, columns, containers)
- **`Container`**: Represents content areas that can hold contentlets
- **`Contentlet`**: Individual pieces of content with their properties

### 3. **Service Layer** (`DotCmsService`)

The service layer handles all communication with the dotCMS API:

```csharp
public async Task<PageResponse> GetPageAsync(PageQueryParams queryParams)
{
    // 1. Build API URL with parameters
    // 2. Add authentication headers
    // 3. Execute HTTP request
    // 4. Deserialize JSON response to C# models
    // 5. Cache response for performance
}
```

Key features:
- **Authentication**: Supports both API token and basic auth
- **Caching**: Uses LazyCache to cache responses (configurable TTL)
- **Error Handling**: Comprehensive logging and exception handling
- **GraphQL Support**: Can query dotCMS via GraphQL as an alternative

For more details, see [DotCmsService Documentation](docs/DotCmsService.md).



### 4. **View Layer** (Razor Templates)

The Razor view engine renders the dotCMS page structure:

#### Main Page View (`Views/DotCmsView/Index.cshtml`)
```razor
@model RazorPagesDotCMS.Models.PageResponse

@foreach (var row in layout.Body.Rows)
{
    <section class="section">
        @foreach (var column in row.Columns)
        {
            <div class="col-lg-@column.Width">
                @foreach (var container in column.Containers)
                {
                    <!-- Render container and its contentlets -->
                    <contentlet-renderer contentlet="contentlet" />
                }
            </div>
        }
    </section>
}
```

The view:
- Iterates through the layout structure (rows → columns → containers)
- Renders Bootstrap-compatible grid markup
- Uses custom TagHelpers to render individual contentlets

#### Content Type Views (`Views/DotCmsView/ContentTypes/`)
Each content type has its own Razor view for custom rendering:

```razor
<!-- Banner.cshtml -->
@model Contentlet
<div class="banner">
    <img src="@Model.GetProperty("image")" alt="@Model.GetProperty("altText")">
    <h2>@Model.Title</h2>
    <p>@Html.Raw(Model.GetProperty("description"))</p>
</div>
```

### 5. **TagHelpers** 

Custom TagHelpers simplify content rendering:

#### ContentletTagHelper
```csharp
[HtmlTargetElement("contentlet-renderer")]
public class ContentletTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // 1. Determine content type
        // 2. Find appropriate view (e.g., Banner.cshtml)
        // 3. Render contentlet using the view
    }
}
```

This allows simple usage in views:
```razor
<contentlet-renderer contentlet="@myContentlet" />
```

#### HeaderTagHelper and FooterTagHelper

These TagHelpers provide reusable header and footer components that can be conditionally displayed:

```cshtml
<!-- Basic usage with default content from partial views -->
<header-section show="@(layout?.Header == true)" title="My Site"></header-section>
<footer-section show="@(layout?.Footer == true)" copyright="My Site"></footer-section>

<!-- With custom content -->
<header-section show="@(layout?.Header == true)">
    <div class="custom-header">
        <h1>Welcome to My Site</h1>
        <nav><!-- Navigation items --></nav>
    </div>
</header-section>
```

## Request Flow

1. **Browser Request**: User navigates to `/about-us`
2. **Controller**: `DotCmsUVEController` catches the request
3. **Service Call**: `DotCmsService.GetPageAsync("/about-us")` is called
4. **API Request**: Service makes HTTP request to dotCMS Page API
5. **Response Processing**: JSON response is deserialized to C# models
6. **Caching**: Response is cached for subsequent requests
7. **View Rendering**: Razor view receives the model and renders HTML
8. **TagHelper Processing**: ContentletTagHelper renders individual content pieces
9. **Response**: Final HTML is sent to the browser

## Configuration

The application is configured through the `appsettings.json` file:

```json
{
  "dotCMS": {
    "ApiHost": "https://demo.dotcms.com",
    "ApiToken": "ABC123......",
    "ApiUserName": "admin@dotcms.com",
    "ApiPassword": "admin",
    "CacheTTL": 120
  },
  "proxy": [
    {
      "enabled": true,
      "path": "/dA*",
      "target": "https://demo.dotcms.com/dA"
    },
    {
      "enabled": true,
      "path": "/contentAsset*",
      "target": "https://demo.dotcms.com/contentAsset"
    }
  ]
}
```

## Free Bonus: Asset/Image Proxy 

The .NET starter contains a static asset proxy that automatically forwards requests that start with `/dA` or `/contentAsset` to dotCMS for servicing.  This is particularly useful when you are running behind a CDN or some other layer and do not want to deal with remembering which origin to use, one for page requests and another for assets and images.  

For more details, see [Proxy Documentation](docs/Proxy.md).

## Documentation

### Service Layer Documentation

- **[DotCmsService](docs/DotCmsService.md)** - Comprehensive documentation for the core service that handles all dotCMS API interactions, including:
  - Configuration and setup
  - Authentication methods (API token vs basic auth)
  - Caching strategies and performance optimization
  - Security features and best practices
  - Usage examples and error handling
  - GraphQL support and custom queries

### Additional Documentation

- **[Proxy Configuration](docs/Proxy.md)** - Details on the asset proxy functionality for serving dotCMS static assets

## Architecture Benefits

This MVC + Razor approach provides several advantages:

1. **Separation of Concerns**: Clear separation between data fetching (Service), request handling (Controller), and presentation (Views)
2. **Type Safety**: Strongly-typed C# models prevent runtime errors
3. **Reusability**: TagHelpers and partial views enable component reuse
4. **Performance**: Built-in caching reduces API calls
5. **Flexibility**: Easy to customize rendering for different content types
6. **Maintainability**: Standard .NET patterns make the code easy to understand

## TODOs

This is a WIP and there is still a lot to do for this example to be complete. These include:

- Transform dotCMS Graphql Page API response so that it renders like the PageAPI does
- Add the required `data-attr` for UVE and containers, content
- Add include the uve-editor.js to "activate" uve when rendered in dotCMS
- Make content type components "container" aware - meaning look for the content type `View` under `/Views/ContentTypes/{containerName}/{ContentType}.cshtml` and then fall back to `/Views/ContentTypes/{ContentType}.cshtml` if a container specific view is not available
