# GWP API - Testing Guide

## Quick Start - Testing the API

### 1. Build the Application

```bash
cd /workspaces/dotnet-codespaces/SampleApp/WebApiConsole
dotnet build
```

### 2. Run the Application

```bash
dotnet run
```

**Expected Output**:
```
Starting self-hosted web API server...
Listening on http://localhost:9091
```

The server is now running and ready to accept requests.

---

## Testing with cURL

### Test 1: Single Line of Business (Transport)

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":["transport"]}'
```

**Expected Response (200 OK)**:
```json
{
    "transport": 451250000.0
}
```

---

### Test 2: Multiple Lines of Business

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":["property","transport","liability"]}'
```

**Expected Response (200 OK)**:
```json
{
    "transport": 451250000.0,
    "property": 1175000.0,
    "liability": 634545022.9
}
```

---

### Test 3: Different Country (United States)

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"us","lob":["transport"]}'
```

**Expected Response (200 OK)**:
```json
{
    "transport": 113625000.0
}
```

---

### Test 4: United Kingdom Data

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"uk","lob":["property","liability"]}'
```

**Expected Response (200 OK)**:
```json
{
    "property": 34375000.0,
    "liability": 164375000.0
}
```

---

### Test 5: Error - Empty Country Code

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"","lob":["transport"]}'
```

**Expected Response (400 Bad Request)**:
```json
{
    "message": "Invalid request data",
    "details": [
        "Country code cannot be empty or null"
    ],
    "traceId": "0HN1GBQV5AOJF:00000001"
}
```

---

### Test 6: Error - Empty Lines of Business

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":[]}'
```

**Expected Response (400 Bad Request)**:
```json
{
    "message": "Invalid request data",
    "details": [
        "At least one line of business must be specified"
    ],
    "traceId": "0HN1GBQV5AOJF:00000002"
}
```

---

## Testing with PowerShell

### Test 1: Single Request

```powershell
$body = @{
    country = "ae"
    lob = @("transport")
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:9091/server/api/gwp/avg" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body

$response | ConvertTo-Json
```

### Test 2: Multiple Requests Loop

```powershell
$requests = @(
    @{ country = "ae"; lob = @("transport") },
    @{ country = "us"; lob = @("liability") },
    @{ country = "uk"; lob = @("property", "transport", "liability") }
)

foreach ($req in $requests) {
    $body = $req | ConvertTo-Json
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:9091/server/api/gwp/avg" `
            -Method Post `
            -ContentType "application/json" `
            -Body $body
        
        Write-Host "Request: $($req | ConvertTo-Json -Compress)"
        Write-Host "Response: $($response | ConvertTo-Json)" -ForegroundColor Green
    } catch {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host "---"
}
```

---

## Testing with Postman

### Setup

1. Open Postman (or install from https://www.postman.com/downloads/)
2. Create a new HTTP request

### Test Request

**Method**: POST

**URL**: `http://localhost:9091/server/api/gwp/avg`

**Headers**:
```
Content-Type: application/json
```

**Body** (raw JSON):
```json
{
    "country": "ae",
    "lob": ["transport", "property"]
}
```

**Click Send** and view the response.

### Save as Collection

1. Click **Save** to save this request
2. Create a collection "GWP API Tests"
3. You can add multiple requests and run them as a suite

---

## Testing with Swagger UI

1. Open browser and navigate to: `http://localhost:9091/swagger/ui`

2. Expand the POST endpoint: **POST /server/api/gwp/avg**

3. Click **Try it out**

4. Enter JSON in the request body:
```json
{
    "country": "ae",
    "lob": ["property", "transport"]
}
```

5. Click **Execute** to send the request

6. View the response below

---

## Testing with Python

```python
import requests
import json

BASE_URL = "http://localhost:9091"

def test_gwp_api(country, lobs):
    """Test the GWP API endpoint"""
    payload = {
        "country": country,
        "lob": lobs
    }
    
    response = requests.post(
        f"{BASE_URL}/server/api/gwp/avg",
        json=payload,
        headers={"Content-Type": "application/json"}
    )
    
    print(f"Country: {country}, LOBs: {lobs}")
    print(f"Status Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2)}")
    print("---\n")

# Test cases
test_gwp_api("ae", ["transport"])
test_gwp_api("ae", ["property", "transport", "liability"])
test_gwp_api("us", ["liability"])
test_gwp_api("uk", ["property"])

# Error case
test_gwp_api("", ["transport"])
```

---

## Running Unit Tests

### Run All Tests

```bash
cd /workspaces/dotnet-codespaces/SampleApp/WebApiConsole.Tests
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter "ClassName=GwpServiceTests"
```

### Run with Detailed Output

```bash
dotnet test --verbosity detailed
```

### Run Specific Test Method

```bash
dotnet test --filter "Name=GetAverageGwp_WithValidRequest_ReturnsAverageValues"
```

---

## Testing Performance & Caching

### First Request (Cache Miss)

```bash
time curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":["transport"]}'
```

Expected: ~50-100ms (reads CSV, calculates, caches result)

### Subsequent Request (Cache Hit)

```bash
time curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -d '{"country":"ae","lob":["transport"]}'
```

Expected: ~1-5ms (returns cached result)

---

## Verifying Cache Behavior

The cache automatically expires after 1 hour. You can verify caching by:

1. Making two identical requests and comparing response times
2. First request will be slower (computes average)
3. Second request will be much faster (returns cached value)
4. Cache key is based on country and sorted LOBs

**Cache Key Format**: `{country}_{sorted_lobs}` (lowercase)

Example: For country "ae" and lobs ["transport", "property"], the cache key would be: `ae_property_transport`

---

## Health Check Endpoint

Test the application is running:

```bash
curl http://localhost:9091/api/health
```

**Response**:
```json
{
    "status": "healthy",
    "port": 9091
}
```

---

## Swagger/OpenAPI Documentation

View the API definition in OpenAPI format:

```bash
curl http://localhost:9091/swagger/v1/swagger.json
```

This returns the complete API specification in OpenAPI 3.0 format, which can be imported into tools like Postman, Insomnia, or used to generate client SDKs.

---

## Test Data Available

### Countries
- `ae` (UAE)
- `us` (USA)
- `uk` (UK)

### Lines of Business
- `property`
- `transport`
- `liability`

### Data Period
- Years 2008-2015 (8 years of data per combination)

**Total Combinations**: 3 countries × 3 LOBs × 8 years = 72 data points

---

## Troubleshooting

### Port 9091 Already in Use

**On Windows**:
```cmd
netstat -ano | findstr :9091
taskkill /PID <PID> /F
```

**On Mac/Linux**:
```bash
lsof -i :9091
kill -9 <PID>
```

### CSV File Not Found

Ensure the working directory is set correctly and `Data/gwp_data.csv` exists in the project root.

### CORS Issues

Add CORS headers if calling from a web browser:

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
  -H "Content-Type: application/json" \
  -H "Origin: http://localhost:3000" \
  -d '{"country":"ae","lob":["transport"]}'
```

### Invalid Request

Verify JSON formatting is correct:
- Country code must not be empty
- LOB array must contain at least one item
- All LOBs must be non-empty strings

---

## Performance Expectations

| Scenario | Latency | Notes |
|----------|---------|-------|
| First Request (Cache Miss) | 50-100ms | Reads CSV, calculates average |
| Cached Request | 1-5ms | Returns cached result |
| Invalid Request | 5-10ms | Validation error response |
| No Data Found | 10-20ms | Empty result set |

---

## Test Results Summary

**Unit Tests**: 22 passing
- Service Layer Tests: 6
- Repository Tests: 5
- Cache Tests: 5
- Controller Tests: 5
- Integration Tests: 1

**Coverage Areas**:
- ✅ Valid request processing
- ✅ Caching functionality
- ✅ Error handling
- ✅ Input validation
- ✅ Case-insensitive matching
- ✅ Multiple LOB handling
- ✅ Empty result sets
