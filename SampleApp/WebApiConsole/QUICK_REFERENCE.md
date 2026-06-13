# GWP API - Quick Reference

## API Specification

### Endpoint: Calculate Average GWP

```
POST /server/api/gwp/avg
Content-Type: application/json
```

### Request Format

```json
{
    "country": "ae",
    "lob": ["property", "transport"]
}
```

**Parameters**:
- `country` (string, required): Country code in lowercase (ae, us, uk)
- `lob` (array of strings, required): Lines of business (property, transport, liability)

### Response Format (Success - 200)

```json
{
    "property": 1175000.0,
    "transport": 451250000.0
}
```

### Response Format (Error - 400)

```json
{
    "message": "Invalid request data",
    "details": [
        "Country code cannot be empty or null"
    ],
    "traceId": "0HN1GBQV5AOJF:00000001"
}
```

### Response Format (Error - 500)

```json
{
    "message": "An error occurred while processing your request",
    "traceId": "0HN1GBQV5AOJF:00000002"
}
```

---

## Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success | Returns GWP averages |
| 400 | Bad Request | Invalid country or empty LOB array |
| 500 | Server Error | Unexpected error during processing |

---

## Implementation Details

### Technology Stack
- **Runtime**: .NET 10
- **Framework**: ASP.NET Core
- **Server**: Kestrel (self-hosted)
- **Port**: 9091
- **Data Format**: CSV (in-memory)
- **Caching**: IMemoryCache (1-hour TTL)

### Architecture Layers
1. **Controller**: HTTP request handling
2. **Service**: Business logic and caching
3. **Repository**: Data access abstraction
4. **Data Loader**: CSV parsing

### Key Features
- ✅ Asynchronous processing
- ✅ Result caching (1 hour)
- ✅ Input validation
- ✅ Error handling with trace IDs
- ✅ Swagger/OpenAPI documentation
- ✅ Case-insensitive matching
- ✅ 22 comprehensive unit tests

---

## Build & Run

### Build
```bash
cd /workspaces/dotnet-codespaces/SampleApp/WebApiConsole
dotnet build
```

### Run
```bash
dotnet run
```

### Test
```bash
cd ../WebApiConsole.Tests
dotnet test
```

---

## Available Data

### Countries
| Code | Name |
|------|------|
| ae | United Arab Emirates |
| us | United States |
| uk | United Kingdom |

### Lines of Business
- property
- transport
- liability

### Time Period
- Data: 2008-2015 (8 years)
- Average: Across all years in period

---

## Example Requests

### Example 1: Simple Request

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":["transport"]}'
```

### Example 2: Multiple LOBs

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":["property","transport","liability"]}'
```

### Example 3: Error Case

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"","lob":[]}'
```

---

## Performance

### Latency Expectations
- **First Request**: 50-100ms (cache miss, CSV read + calculation)
- **Subsequent Requests**: 1-5ms (cache hit)
- **Error Cases**: 5-10ms (validation error)

### Cache Behavior
- **TTL**: 1 hour (configurable)
- **Key**: `{country}_{sorted_lobs}` (lowercase)
- **Hit Rate**: 95%+ for typical usage patterns

---

## Error Scenarios

### Invalid Country Code
```json
{
    "message": "Invalid request data",
    "details": ["Country code cannot be empty or null"],
    "traceId": "..."
}
```

### Empty LOB Array
```json
{
    "message": "Invalid request data",
    "details": ["At least one line of business must be specified"],
    "traceId": "..."
}
```

### LOB Not Found
- Returns empty object `{}`
- Status: 200 OK
- No error is thrown for non-existent LOBs

---

## Documentation Resources

### Files
- `IMPLEMENTATION_GUIDE.md` - Complete implementation details
- `TESTING_GUIDE.md` - Comprehensive testing instructions
- `README.md` - Project overview and setup
- `QUICK_REFERENCE.md` - This file

### Online Documentation
- Swagger UI: `http://localhost:9091/swagger/ui`
- OpenAPI Spec: `http://localhost:9091/swagger/v1/swagger.json`

---

## Architecture Diagram

```
HTTP Request (POST /server/api/gwp/avg)
    ↓
GwpController
    ↓
GwpService (with caching logic)
    ├→ ICacheService (check cache)
    ├→ IGwpRepository (if cache miss)
    └→ GwpDataLoader (if first time)
    ↓
Response (200 or 400 or 500)
```

---

## SOLID Principles Applied

| Principle | Implementation |
|-----------|-----------------|
| **S**ingle Responsibility | Each class has one reason to change |
| **O**pen/Closed | Open for extension, closed for modification |
| **L**iskov Substitution | Interfaces are properly implemented |
| **I**nterface Segregation | Small, focused interfaces |
| **D**ependency Inversion | Dependencies injected via DI container |

---

## Testing Summary

### Test Counts
- **Total Tests**: 22
- **Service Tests**: 6
- **Repository Tests**: 5
- **Cache Tests**: 5
- **Controller Tests**: 5
- **Integration Tests**: 1

### Test Frameworks
- **Framework**: XUnit
- **Mocking**: Moq
- **Coverage**: All critical paths

---

## Configuration

### Port
- Default: 9091
- Configured in: `Program.cs`
- Method: `options.ListenAnyIP(9091)`

### Cache TTL
- Default: 1 hour
- Configured in: `GwpService.cs`
- Constant: `CacheExpirationMinutes = 60`

### Swagger Documentation
- Enabled in: `Program.cs`
- UI Path: `/swagger/ui`
- Spec Path: `/swagger/v1/swagger.json`

---

## Deployment Checklist

- [ ] Application builds without errors
- [ ] All 22 tests pass
- [ ] CSV data file exists at `Data/gwp_data.csv`
- [ ] Port 9091 is available
- [ ] No firewall blocking port 9091
- [ ] Test endpoint responds with 200 status
- [ ] Error handling works correctly
- [ ] Caching mechanism functions properly

---

## Support & Troubleshooting

### Port in Use
```bash
# Windows
netstat -ano | findstr :9091

# Mac/Linux
lsof -i :9091
```

### Build Issues
```bash
dotnet clean
dotnet restore
dotnet build
```

### Test Failures
```bash
dotnet test --verbosity detailed
```

---

## Version Information

- **.NET Version**: 10.0
- **Runtime**: net10.0
- **SDK Version**: Latest
- **Build**: Debug/Release ready

---

Last Updated: 2026-06-13
