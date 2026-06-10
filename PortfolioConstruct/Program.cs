using PortfolioConstruct.Components;
using PortfolioConstruct.Models;
using PortfolioConstruct.Repositories;
using PortfolioConstruct.Services;
using PortfolioConstruct.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Repositories =====
builder.Services.AddScoped<IUserRepository,           UserRepository>();
builder.Services.AddScoped<IPortfolioRepository,      PortfolioRepository>();
builder.Services.AddScoped<ISectionRepository,        SectionRepository>();
builder.Services.AddScoped<IBlockRepository,          BlockRepository>();
builder.Services.AddScoped<IBlockTypeRepository,      BlockTypeRepository>();
builder.Services.AddScoped<IDesignRepository,         DesignRepository>();
builder.Services.AddScoped<IGalleryRepository,        GalleryRepository>();
builder.Services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();

// ===== Services =====
builder.Services.AddScoped<IAuthService,           AuthService>();
builder.Services.AddScoped<IPortfolioService,      PortfolioService>();
builder.Services.AddScoped<IFileService,           FileService>();
builder.Services.AddScoped<IAdminService,          AdminService>();
builder.Services.AddScoped<IStudentProfileService, StudentProfileService>();
builder.Services.AddScoped<HtmlExportService>();

builder.Services.AddScoped<SessionService>();

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
