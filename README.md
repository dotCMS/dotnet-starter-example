![dotcms-logo-square](https://github.com/user-attachments/assets/97d16db7-706c-4f4c-b352-619e1c8ceccc) &nbsp; &nbsp; &nbsp; &nbsp; ![Microsoft_ NET_logo](https://github.com/user-attachments/assets/59b19db7-206e-49c4-83b1-efbaf98ca07f)  &nbsp; 


# dotCMS .NET Example for UVE

This project is a UVE .NET example/base project and is intended to be used as a template for web projects looking to render dotCMS content and pages using [.NET MVC](https://dotnet.microsoft.com/en-us/apps/aspnet/mvc) and [Razor templates](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/).  The example points to our demo site and is pulling the page, content, layout grid and other information from https://demo.dotcms.com


## Features

- Render dotCMS content and pages in a .NET application
- Uses C#, .NET MVC and Razor templates for views
- Service-based architecture for API interactions





## Configuration

The application is configured through the `appsettings.json` file:

```json
{
  "dotCMS": {
    "ApiHost": "https://demo.dotcms.com",
    "ApiToken": "ABC123......",
    "ApiUserName": "admin@dotcms.com",
    "ApiPassword": "admin"
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

### Proxy Configuration

The `proxy` section configures the proxy feature, which forwards specific requests to dotCMS or other target servers. This is particularly useful for handling assets, images, and other resources that need to be served directly from dotCMS.

Each proxy configuration has the following properties:
- `enabled`: Boolean to enable/disable the proxy
- `path`: The URI pattern to match (supports wildcards with *)
- `target`: The target server URL to proxy to

For more details, see [Proxy Documentation](docs/Proxy.md).

You can authenticate using either:
- An API token (preferred for production)
- Username and password

## Architecture

The application follows a clean architecture pattern:

1. **Models**: Data models representing dotCMS entities (Page, Layout, Container, etc.)
2. **Services**: Services for interacting with the dotCMS API
3. **Controllers**: Controllers for handling HTTP requests
4. **Views**: Razor views for rendering dotCMS pages

### Key Components

- **DotCmsService**: Service for interacting with the dotCMS API.  This includes methods to call dotCMS APIs and the dotCMS graphQL endpoint.
- **DotCmsUVEController**: Controller for handling dotCMS page requests
- **ProxyActionFilter**: Action filter that proxies requests to dotCMS or other target servers based on configured paths



## Usage

The DotCmsUVEController acts as a catchall/proxy for dotCMS pages. When a request is made to any path, the `DotCmsUVEController` will:

1. Forward the uri to the dotCMS Page API to get the Page response, which includes all data regarding the template, layout, content blocks and content and which can be used to composit a page.
2. Parse the response into the appropriate models
3. Render the page using the Razor view
4. Caches the PageAPI response for up to 1 minute for performance.

### TagHelpers

The SDK includes custom TagHelpers to simplify common UI components:


#### ContentletTagHelper
This tag helper accepts a dotCMS contentlet object and passes the content data to a view for appropiate rendering and inclusion on a page.  It accepts a Contentlet and based upon that Contentlet's content type, will look for an appropiate `View` file under the `/Views/ContentTypes` to use to render the content. 

Things that could be improved - perhaps the ContentletTagHelper should also take the 

#### HeaderTagHelper and FooterTagHelper

These TagHelpers provide reusable header and footer components that can be conditionally displayed. The default content is defined in partial views:

- `Views/Shared/_HeaderExample.cshtml` - Default header content
- `Views/Shared/_FooterExample.cshtml` - Default footer content

Usage examples:

```cshtml
<!-- Basic usage with default content from partial views -->
<header-section show="@(layout?.Header == true)" title="My Site"></header-section>
<footer-section show="@(layout?.Footer == true)" copyright="My Site"></footer-section>

<!-- With custom content -->
<header-section show="@(layout?.Header == true)">
    <div class="custom-header">
        <h1>Welcome to My Site</h1>
        <nav>
            <ul>
                <li><a href="/">Home</a></li>
                <li><a href="/about">About</a></li>
            </ul>
        </nav>
    </div>
</header-section>
```

Key features:
- Conditional rendering based on the `show` attribute
- Default content loaded from partial views
- Support for custom content when provided between tags
- Customizable through attributes (`title` for header, `copyright` for footer)

See `Views/Shared/_TagHelperExamples.cshtml` for more detailed examples.

## Development

To run the application locally:

```bash
dotnet run
```

The application will be available at `https://localhost:5001`.



## TODOs
This is a WIP and there is still a lot to do for this example to be complete.  These include:


- Transform dotCMS Graphql Page API response so that it renders like the PageAPI does.
- Add the required `data-attr` for UVE and containers, content
- Add include the uve-editor.js to "activate" uve when rendered in dotCMS
- Make content type components "container" aware - meaning look for the content type `View` under `/Views/ContentTypes/{containerName}/{ContentType}.cshtml` and then fall back to `/Views/ContentTypes/{ContentType}.cshtml` if a container specific view is not available.
