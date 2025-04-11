using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWebApp.Pages
{
    public class DotCMSPageModel : PageModel
    {
        private readonly ILogger<DotCMSPageModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DotCMSPageModel(ILogger<DotCMSPageModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public PageData PageContent { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var token = "PUT_YOUR_TOKEN_HERE"
                    ?? throw new InvalidOperationException("Missing API token in configuration");

                var url = "http://localhost:8080/api/v1/page/json/";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var responseData = JsonSerializer.Deserialize<ApiResponse>(jsonString, options);
                PageContent = responseData.Entity;
                // Console.WriteLine(jsonString);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading page data");
                return StatusCode(500, "Error loading page: " + ex.Message);
            }
        }
    }

    public class ApiResponse
    {
        public PageData Entity { get; set; }
    }

    public class PageData
    {
        public PageInfo Page { get; set; }
        public LayoutData Layout { get; set; }
        public Dictionary<string, ContainerData> Containers { get; set; }
    }

    public class PageInfo
    {
        public string FriendlyName { get; set; }
    }

    public class LayoutData
    {
        public BodyData Body { get; set; }
    }

    public class BodyData
    {
        public List<RowData> Rows { get; set; }
    }

    public class RowData
    {
        public List<ColumnData> Columns { get; set; }
        public string StyleClass { get; set; }
    }

    public class ColumnData
    {
        public List<ContainerRef> Containers { get; set; }
        public int Left { get; set; }
        public int LeftOffset { get; set; }
        public bool Preview { get; set; }
        public string StyleClass { get; set; }
        public int Width { get; set; }
        public int WidthPercent { get; set; }
    }

    public class ContainerRef
    {
        public List<string> HistoryUUIDs { get; set; }
        public string Identifier { get; set; }
        public string UUID { get; set; }
    }

    public class ContainerData
    {
        public Container Container { get; set; }
        public List<ContainerStructure> ContainerStructures { get; set; }
        public Dictionary<string, List<Contentlet>> Contentlets { get; set; }
    }

    public class Container 
    {
        public int maxContentlets { get; set; }
        public string variantId { get; set; }
    }

    public class ContainerStructure
    {
        public string ContentTypeVar { get; set; }
    }

    public class Contentlet
    {
        public string Identifier { get; set; }
        public string BaseType { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string WidgetTitle { get; set; }
        public string Inode { get; set; }
        public string Image { get; set; }
        public string Caption { get; set; }
        public string ButtonText { get; set; }
        public string Link { get; set; }
        public string RetailPrice { get; set; }
        public string SalePrice { get; set; }
        public string UrlTitle { get; set; }
        public string Description { get; set; }
    }
} 