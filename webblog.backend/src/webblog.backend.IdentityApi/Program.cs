using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using webblog.backend.IdentityApi.Abstractions;
using webblog.backend.IdentityApi.Data;
using webblog.backend.IdentityApi.Models;
using webblog.backend.IdentityApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebBlog Identity API"
    });
});

builder.Services.AddConnections(builder.Configuration["DbNpgsql"]);

var environment = builder.Services.BuildServiceProvider()
    .GetRequiredService<IWebHostEnvironment>();
builder.Services.AddDataProtection()
                .SetApplicationName($"webblog-{environment.EnvironmentName}")
                .PersistKeysToFileSystem(new DirectoryInfo($@"{environment.ContentRootPath}\keys"));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@+";
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<ApplicationRole>>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/account/login";
        options.LogoutPath = "/api/account/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
    });

string corsPolicy = "main_origins";
var origins = builder.Configuration.GetSection("Origins").Get<string[]>();
builder.Services.AddCors(o => o.AddPolicy(corsPolicy, builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins(origins ?? new[] { "localhost" });
}));

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.UseAuthentication();
app.UseCors(corsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/api/identity/health");

app.UseAuthorization();

app.MapControllers();

app.Run();
