// File: Program.cs
// Path: BudgetBuddy/Program.cs

using BudgetBuddy.Data;
using BudgetBuddy.Models;
using BudgetBuddy.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages + Blazor Server
builder.Services.AddRazorPages();      // ✅ For .cshtml Razor Pages
builder.Services.AddServerSideBlazor(); // ✅ For Blazor Components

// ✅ Configure EF Core with PostgreSQL (Neon DB)
builder.Services.AddDbContext<FinancyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Configure ASP.NET Core Identity with our User model
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<FinancyContext>();

// ✅ Add AccountService (for any remaining Blazor components)
builder.Services.AddScoped<AccountService>();

var app = builder.Build();

// ✅ Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map both Razor Pages and Blazor
app.MapRazorPages();  // ✅ Enables /Pages/*.cshtml routes
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// ✅ Seed initial categories if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();