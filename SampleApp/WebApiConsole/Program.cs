using WebApiConsole.Caching;
using WebApiConsole.Data;
using WebApiConsole.Repositories;
using WebApiConsole.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 9091
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(9091);
});

// Add services to the container
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
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add memory cache
builder.Services.AddMemoryCache();

// Register custom services
builder.Services.AddSingleton<GwpDataLoader>();
builder.Services.AddScoped<IGwpRepository, GwpRepository>();
builder.Services.AddScoped<IGwpService, GwpService>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Sample endpoint
app.MapGet("/", () => new { message = "Self-hosted Web API Server running on port 9091", timestamp = DateTime.UtcNow })
    .WithName("GetRoot");

app.MapGet("/api/health", () => new { status = "healthy", port = 9091 })
    .WithName("GetHealth");

Console.WriteLine("Starting self-hosted web API server...");
Console.WriteLine("Listening on http://localhost:9091");
await app.RunAsync();
