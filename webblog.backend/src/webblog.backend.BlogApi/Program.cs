using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using webblog.backend.BlogApi.Abstractions;
using webblog.backend.BlogApi.Data;
using webblog.backend.BlogApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebBlog Blog API"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("main_origins", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(builder.Configuration.GetSection("Origins").Get<string[]>() ?? new[] { "localhost" });
    });
});

builder.Services.AddConnections(builder.Configuration["DbNpgsql"] ?? "test");

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddScoped<IPostService, PostService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("main_origins");

if (app.Environment.IsDevelopment()) { }

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/api/blog/health");

app.UseAuthorization();

app.MapControllers();

app.Run();
