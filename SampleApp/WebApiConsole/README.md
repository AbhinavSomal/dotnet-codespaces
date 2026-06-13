# Self-Hosted Web API Console Application

A .NET 10 console application that runs a self-hosted ASP.NET Core web API server using Kestrel, listening on **port 9091**.

## Features

- **Console Application**: Runs as a simple console app with no external dependencies
- **Kestrel Server**: Built-in self-hosted web server
- **ASP.NET Core API**: Full support for controllers, minimal APIs, and dependency injection
- **.NET 10**: Latest .NET runtime
- **Swagger/OpenAPI**: Built-in API documentation (in Development environment)

## Project Structure

```
WebApiConsole/
├── Program.cs                 # Entry point and configuration
├── WebApiConsole.csproj       # Project file
├── appsettings.json          # Configuration
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json
└── Controllers/
    └── ExampleController.cs   # Sample API controller
```

## Running the Application

### Build
```bash
dotnet build WebApiConsole.csproj
```

### Run
```bash
dotnet run --project WebApiConsole.csproj
```

The server will start and listen on `http://localhost:9091`

### Available Endpoints

- **GET** `/` - Root endpoint returning server info
- **GET** `/api/health` - Health check endpoint
- **GET** `/api/example` - Example controller endpoint
- **POST** `/api/example` - Post example data
- **GET** `/swagger/ui` - Swagger UI (Development only)

## Configuration

### Port Configuration
The port is configured in `Program.cs`:
```csharp
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(9091);
});
```

To change the port, modify the number `9091` in the UseKestrel configuration.

### Environment

The application respects the `ASPNETCORE_ENVIRONMENT` variable:
- `Development` - Enables Swagger, detailed logging
- `Production` - Minimal logging, no Swagger

## Adding New Endpoints

### Using Controllers
Add new controllers in the `Controllers/` folder and they'll be automatically discovered and registered.

### Using Minimal APIs
Add minimal APIs directly in `Program.cs`:
```csharp
app.MapGet("/api/custom", () => "Hello from custom endpoint");
```

## Adding Dependencies

To add NuGet packages:
```bash
cd WebApiConsole
dotnet add package PackageName
```

## Extending the Application

- Add database connectivity (EF Core, Dapper, etc.)
- Implement authentication/authorization (JWT, OAuth, etc.)
- Add middleware for logging, CORS, rate limiting
- Configure HTTPS for production
- Add unit/integration tests
