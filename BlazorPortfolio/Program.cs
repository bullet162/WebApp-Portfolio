using BlazorPortfolio.Components;
using BlazorPortfolio.Data;
using BlazorPortfolio.Models;
using BlazorPortfolio.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ContentService>();
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<GitHubService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHostedService<KeepAliveService>();

var app = builder.Build();

// Auto-migrate on startup and seed default admin if none exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.AdminUsers.Any())
    {
        db.AdminUsers.Add(new AdminUser
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
        });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
