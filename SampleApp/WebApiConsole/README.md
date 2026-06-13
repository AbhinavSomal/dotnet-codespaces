# Self-Hosted Web API Console Application

A .NET 10 console application that runs a self-hosted ASP.NET Core web API server using Kestrel, listening on **port 9091**.

## Features

- **Console Application**: Runs as a simple console app with no external dependencies
- **Swagger/OpenAPI**: Built-in API documentation

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
    └── CountryGwpController.cs   # Controller
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
- **GET** `/swagger/ui` - Swagger UI (not working currently)

## Configuration


## Extending the Application

- Add database connectivity (EF Core, Dapper, etc.)
- Implement authentication/authorization (JWT, OAuth, etc.)
- Add middleware for logging, CORS, rate limiting
- Add integration tests
- Create Service Components (Application SC, Function SC, Entity SC) for business logic while extending feature further.
