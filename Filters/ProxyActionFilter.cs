using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RazorPagesDotCMS.Models;

namespace RazorPagesDotCMS.Filters
{
    public class ProxyActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<ProxyActionFilter> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly List<ProxyConfig> _proxyConfigs;

        public ProxyActionFilter(
            ILogger<ProxyActionFilter> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            
            // Load proxy configurations from appsettings.json
            _proxyConfigs = _configuration.GetSection("proxy").Get<List<ProxyConfig>>() ?? new List<ProxyConfig>();
            _logger.LogInformation($"Loaded {_proxyConfigs.Count} proxy configurations");
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var path = request.Path.Value ?? string.Empty;
            
            _logger.LogInformation($"Processing request path: {path}");

            // Check if the request path matches any of the proxy configurations
            var matchingProxy = _proxyConfigs.FirstOrDefault(p => p.IsMatch(path));
            
            if (matchingProxy == null)
            {
                _logger.LogInformation($"No proxy match found for {path}, continuing to controller");
                // No match found, continue to the controller
                await next();
                return;
            }

            _logger.LogInformation($"Proxy match found for {path}, proxying to {matchingProxy.Target}");
            
            // Create a proxy request
            var httpClient = _httpClientFactory.CreateClient("ProxyClient");
            var targetUri = new Uri(matchingProxy.Target + path.Substring(matchingProxy.Path.Replace("*", "").Length));
            
            //_logger.LogInformation($"Proxying request to {targetUri}");

            // Create the HttpRequestMessage
            var proxyRequest = new HttpRequestMessage();
            proxyRequest.Method = new HttpMethod(request.Method);
            proxyRequest.RequestUri = targetUri;

            // Copy headers from the original request
            foreach (var header in request.Headers)
            {
                if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Copy the request body if present
            if (request.ContentLength > 0)
            {
                var streamContent = new StreamContent(request.Body);
                proxyRequest.Content = streamContent;
                
                // Copy content headers
                if (request.ContentType != null)
                {
                    proxyRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(request.ContentType);
                }
            }

            try
            {
                // Send the proxy request
                var proxyResponse = await httpClient.SendAsync(proxyRequest);
                
                // Copy the response status code
                context.HttpContext.Response.StatusCode = (int)proxyResponse.StatusCode;
                
                // Copy the response headers
                foreach (var header in proxyResponse.Headers)
                {
                    context.HttpContext.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // Copy the response content headers
                if (proxyResponse.Content != null)
                {
                    foreach (var header in proxyResponse.Content.Headers)
                    {
                        context.HttpContext.Response.Headers[header.Key] = header.Value.ToArray();
                    }

                    // Copy the response body
                    var responseStream = await proxyResponse.Content.ReadAsStreamAsync();
                    await responseStream.CopyToAsync(context.HttpContext.Response.Body);
                }

                // Short-circuit the pipeline
                context.Result = new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error proxying request to {targetUri}");
                context.Result = new StatusCodeResult((int)HttpStatusCode.BadGateway);
            }
        }
    }
}
