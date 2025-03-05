# dotCMS .NET SDK

This project is a .NET SDK for interacting with dotCMS. It provides a way to render dotCMS pages in a .NET application.

## Features

- Render dotCMS pages in a .NET application
- Support for container instances with proper UUID handling
- Service-based architecture for API interactions

## Configuration

The application is configured through the `appsettings.json` file:

```json
{
  "dotCMS": {
    "ApiHost": "https://demo.dotcms.com",
    "ApiToken": "",
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

- **DotCmsService**: Service for interacting with the dotCMS API
- **DotCmsUVEController**: Controller for handling dotCMS page requests
- **Container Model**: Enhanced to maintain UUID information for container instances

## Container Instances

One of the key features is the ability to handle multiple instances of the same container on a page. Each container instance has its own UUID, and the SDK ensures that contentlets are properly filtered by container instance.

The `Container` class in `Layout.cs` has a `ContainerInstanceId` property that formats the UUID as "uuid-X" where X is the value of the Uuid property. This property is used to match against the keys in the `Contentlets` dictionary.

## Usage

The application acts as a proxy for dotCMS pages. When a request is made to any path, the `DotCmsUVEController` will:

1. Forward the request to the dotCMS API
2. Parse the response into the appropriate models
3. Render the page using the Razor view

## Development

To run the application locally:

```bash
dotnet run
```

The application will be available at `https://localhost:5001`.
