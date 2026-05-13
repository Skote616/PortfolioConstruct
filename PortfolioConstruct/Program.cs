using PortfolioConstruct.Components;
using PortfolioConstruct.Models;
using PortfolioConstruct.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// Razor + Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Отключаем проверку antiforgery-токена — мы не используем HTML <form>,
// только Blazor @onclick. Но сам middleware оставляем — .NET 8 его требует.
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

// DbContext через DI
builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DBService и SessionService — Scoped (один экземпляр на подключение)
builder.Services.AddScoped<DBService>();
builder.Services.AddScoped<SessionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery(); // обязателен в .NET 8, но страницы мы не помечаем [ValidateAntiForgeryToken]

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
