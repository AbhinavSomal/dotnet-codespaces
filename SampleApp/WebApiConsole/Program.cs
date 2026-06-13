using WebApiConsole.Caching;
using WebApiConsole.Data;
using WebApiConsole.Repositories;
using WebApiConsole.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(9091);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Country GWP API",
        Version = "v1",
        Description = "API for calculating average Gross Written Premium (GWP)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "GWP API Support"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<GwpDataLoader>();
builder.Services.AddScoped<IGwpRepository, GwpRepository>();
builder.Services.AddScoped<IGwpService, GwpService>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Country GWP API v1");
    c.RoutePrefix = "swagger"; // UI served at /swagger (default)
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => new { message = "Self-hosted Web API Server running on port 9091", timestamp = DateTime.UtcNow })
    .WithName("GetRoot");

app.MapGet("/api/health", () => new { status = "healthy", port = 9091 })
    .WithName("GetHealth");

Console.WriteLine("Starting self-hosted web API server...");
Console.WriteLine("Listening on http://localhost:9091");
await app.RunAsync();
