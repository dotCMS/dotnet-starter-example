var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); // Changed from AddControllers to AddControllersWithViews
builder.Services.AddHttpClient();

// Register named HttpClient for proxy
builder.Services.AddHttpClient("ProxyClient", client => {
    client.DefaultRequestHeaders.Add("User-Agent", "DotCMS.NET-SDK-Proxy");
    client.Timeout = TimeSpan.FromSeconds(60);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
{
    UseCookies = false
});

// Register ProxyActionFilter
builder.Services.AddScoped<RazorPagesDotCMS.Filters.ProxyActionFilter>();

// Register LazyCache
builder.Services.AddSingleton<LazyCache.IAppCache>(new LazyCache.CachingService());

// Register ModelHelper
builder.Services.AddScoped<RazorPagesDotCMS.Models.ModelHelper>();

// Register DotCMS service with HttpClient that doesn't use cookies
builder.Services.AddHttpClient<RazorPagesDotCMS.Services.IDotCmsService, RazorPagesDotCMS.Services.DotCmsService>(client => {
    client.DefaultRequestHeaders.Add("User-Agent", "DotCMS.NET-SDK");
    client.Timeout = TimeSpan.FromSeconds(60);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
{
    UseCookies = false
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
