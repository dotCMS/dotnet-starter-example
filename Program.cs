var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); // Changed from AddControllers to AddControllersWithViews
builder.Services.AddHttpClient();

// Register named HttpClient for proxy
builder.Services.AddHttpClient("ProxyClient", client => {
    client.DefaultRequestHeaders.Add("User-Agent", "DotCMS.NET-SDK-Proxy");
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Register ProxyActionFilter
builder.Services.AddScoped<RazorPagesDotCMS.Filters.ProxyActionFilter>();

// Register LazyCache
builder.Services.AddSingleton<LazyCache.IAppCache>(new LazyCache.CachingService());

// Register DotCMS service
builder.Services.AddScoped<RazorPagesDotCMS.Services.IDotCmsService, RazorPagesDotCMS.Services.DotCmsService>();

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
