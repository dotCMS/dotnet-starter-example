# DotCMS .NET SDK Proxy Feature

The DotCMS .NET SDK includes a proxy feature that allows you to forward specific requests to DotCMS or other target servers. This is particularly useful for handling assets, images, and other resources that need to be served directly from DotCMS.

## How It Works

The proxy feature uses an ASP.NET Core Action Filter that intercepts requests to the DotCMSUVEController. When a request matches one of the configured proxy paths, the request is forwarded to the target server. If the request doesn't match any proxy paths, it's handled by the controller as usual.

## Configuration

The proxy is configured in the `appsettings.json` file under the `proxy` section. Each proxy configuration has the following properties:

- `enabled`: Boolean to enable/disable the proxy
- `path`: The URI pattern to match (supports wildcards with *)
- `target`: The target server URL to proxy to

Example configuration:

```json
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
```

## Adding New Proxy Rules

To add a new proxy rule, simply add a new entry to the `proxy` array in `appsettings.json`:

```json
{
  "enabled": true,
  "path": "/your-path*",
  "target": "https://your-target-server.com/your-path"
}
```

The `path` property supports wildcards using the `*` character. For example, `/dA*` will match any path that starts with `/dA`.

## Implementation Details

The proxy feature is implemented using the following components:

1. `ProxyConfig` class: Represents a proxy configuration from appsettings.json
2. `ProxyActionFilter`: An ASP.NET Core Action Filter that handles the proxying logic
3. `ProxyAttribute`: An attribute that applies the ProxyActionFilter to controllers

The ProxyActionFilter is registered as a scoped service in the dependency injection container, and a named HttpClient is configured for making the proxy requests.

## Troubleshooting

If you're having issues with the proxy feature, check the logs for messages from the `ProxyActionFilter`. The filter logs information about the request path, whether it matched a proxy configuration, and the target URL it's proxying to.

Common issues:

1. **Request not being proxied**: Make sure the path in your proxy configuration matches the request path. The matching is case-insensitive and uses regex pattern matching.

2. **Proxy target not reachable**: Ensure the target server is accessible from your application and that the URL is correct.

3. **Headers or content not being properly forwarded**: The proxy forwards most headers and the request body, but some headers like `Host` are not forwarded to avoid conflicts.
