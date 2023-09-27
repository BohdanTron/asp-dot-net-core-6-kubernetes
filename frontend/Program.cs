using GloboTicket.Frontend.Services;
using GloboTicket.Frontend.Models;
using GloboTicket.Frontend.Services.Ordering;
using GloboTicket.Frontend.Services.ShoppingBasket;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IShoppingBasketService, InMemoryShoppingBasketService>();
builder.Services.AddHttpClient<IConcertCatalogService, ConcertCatalogService>(
    (provider, client) =>
    {
        var uri = provider.GetService<IConfiguration>()?["ApiConfigs:ConcertCatalog:Uri"];
        client.BaseAddress = new Uri(uri ?? throw new InvalidOperationException("Missing config"));
    });

builder.Services.AddHttpClient<IOrderSubmissionService, HttpOrderSubmissionService>(
    (provider, client) =>
    {
        var uri = provider.GetService<IConfiguration>()?["ApiConfigs:Ordering:Uri"];
        client.BaseAddress = new Uri(uri ?? throw new InvalidOperationException("Missing config"));
    });

builder.Services.AddSingleton<Settings>();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Turning this off to simplify the running in Kubernetes demo
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ConcertCatalog}/{action=Index}/{id?}");

app.Run();
