using PortfolioConstruct.Components;
using PortfolioConstruct.Models;
using PortfolioConstruct.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// Razor + Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Antiforgery обязателен в .NET 8 — убираем только проверку X-Frame заголовка
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

// DbContext — Scoped, один экземпляр на подключение
builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Сервисы приложения
builder.Services.AddScoped<DBService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<HtmlExportService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
