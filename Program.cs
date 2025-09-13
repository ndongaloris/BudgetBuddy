
// program.cs

using Microsoft.EntityFrameworkCore;
using Financy.Data;
using Microsoft.AspNetCore.Identity;
using Financy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using AspNet.Security.OAuth.GitHub; // ✅ Needed for GitHub auth

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<FinancyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FinancyContext")));

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<FinancyContext>()
.AddDefaultTokenProviders();

// Add External Authentication (Google + GitHub)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
        var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrEmpty(googleClientId) || string.IsNullOrEmpty(googleClientSecret))
        {
            throw new InvalidOperationException("Google authentication keys are missing in configuration.");
        }

        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.CallbackPath = "/signin-google";
        options.SaveTokens = true;

        // Map Google claims to user properties
        options.ClaimActions.MapJsonKey("picture", "picture");
        options.ClaimActions.MapJsonKey("email_verified", "email_verified");
    })
    .AddGitHub(options =>
    {
        var githubClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        var githubClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];

        if (string.IsNullOrEmpty(githubClientId) || string.IsNullOrEmpty(githubClientSecret))
        {
            throw new InvalidOperationException("GitHub authentication keys are missing in configuration.");
        }

        options.ClientId = githubClientId;
        options.ClientSecret = githubClientSecret;
        options.CallbackPath = "/signin-github";
        options.SaveTokens = true;

        // GitHub requires requesting email scope
        options.Scope.Add("user:email");

        // Map GitHub claims to user properties
        options.ClaimActions.MapJsonKey("avatar_url", "picture");
        options.ClaimActions.MapJsonKey("name", "name");
        options.ClaimActions.MapJsonKey("login", "username");
    });

// Configure application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
