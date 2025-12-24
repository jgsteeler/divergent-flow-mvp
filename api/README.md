# Divergent Flow API

ADHD-friendly brain management tool - Backend Web API

## Overview

This is a .NET 9 Web API that provides backend services for the Divergent Flow application. The API provides CRUD operations for managing capture items.

## Features

- ✅ RESTful API endpoints for capture management
- ✅ Swagger/OpenAPI documentation
- ✅ CORS enabled for frontend integration
- ✅ Dependency injection architecture
- ✅ In-memory data storage (temporary, will be replaced with database)
- ✅ Zero authentication (will be added in future phases)

## Tech Stack

- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core Web API** - Web API framework
- **Swashbuckle** - Swagger/OpenAPI documentation
- **xUnit** - Testing framework
- **C# 13** - Programming language

## Getting Started

### Prerequisites

- .NET 10 SDK or later
- Any IDE (Visual Studio, VS Code, Rider, etc.)

### Running the API

```bash
cd api/DivergentFlow.Api
dotnet run
```

The API will start at `http://localhost:5100` by default.

### Accessing Swagger UI

Once the API is running, navigate to:
```
http://localhost:5100/swagger
```

This provides an interactive UI to test all API endpoints.

## API Endpoints

### Captures

#### GET /api/captures
Get all captures
- **Response**: `200 OK` with array of `CaptureDto`

#### GET /api/captures/{id}
Get a specific capture by ID
- **Parameters**: `id` (string) - Capture ID
- **Response**: `200 OK` with `CaptureDto` or `404 Not Found`

#### POST /api/captures
Create a new capture
- **Request Body**: `CreateCaptureRequest`
  ```json
  {
    "text": "Your capture text here"
  }
  ```
- **Response**: `201 Created` with `CaptureDto`

#### PUT /api/captures/{id}
Update an existing capture
- **Parameters**: `id` (string) - Capture ID
- **Request Body**: `UpdateCaptureRequest`
  ```json
  {
    "text": "Updated capture text"
  }
  ```
- **Response**: `200 OK` with `CaptureDto` or `404 Not Found`

#### DELETE /api/captures/{id}
Delete a capture
- **Parameters**: `id` (string) - Capture ID
- **Response**: `204 No Content` or `404 Not Found`

## Data Models

### CaptureDto
```csharp
{
  "id": "string",           // UUID
  "text": "string",         // Capture content
  "createdAt": 1234567890   // Unix timestamp in milliseconds
}
```

## Project Structure

```
api/
├── DivergentFlow.Api/
│   ├── Controllers/
│   │   └── CapturesController.cs    # API endpoints
│   ├── Models/
│   │   ├── CaptureDto.cs            # Data transfer object
│   │   ├── CreateCaptureRequest.cs  # Create request model
│   │   └── UpdateCaptureRequest.cs  # Update request model
│   ├── Services/
│   │   ├── ICaptureService.cs       # Service interface
│   │   └── InMemoryCaptureService.cs # Temporary in-memory implementation
│   └── Program.cs                    # Application startup
├── DivergentFlow.Api.Tests/
│   └── CapturesControllerTests.cs   # Integration tests
├── DivergentFlow.sln                 # Solution file
└── version.txt                       # Version tracking for releases
```

## Architecture

The API follows a clean architecture pattern:

1. **Controllers** - Handle HTTP requests and responses
2. **Services** - Business logic layer (injected via DI)
3. **Models** - Data transfer objects (DTOs)

### Dependency Injection

Services are registered in `Program.cs`:

```csharp
builder.Services.AddSingleton<ICaptureService, InMemoryCaptureService>();
```

This allows for easy swapping of implementations (e.g., replacing in-memory storage with database).

## CORS Configuration

CORS is configured to allow requests from the frontend:
- `http://localhost:5173` (Vite dev server)
- `http://localhost:5000` (Alternative port)

## Future Enhancements

- [ ] Database integration (PostgreSQL or SQL Server)
- [ ] Authentication & Authorization
- [ ] Type inference endpoints
- [ ] Property validation endpoints
- [ ] LLM integration for intelligent processing
- [ ] Rate limiting
- [ ] Logging and monitoring
- [ ] Unit and integration tests

## Development

### Building the Project

```bash
dotnet build
```

### Running Tests (when added)

```bash
dotnet test
```

Tests are located in `DivergentFlow.Api.Tests` and include:
- Integration tests for all API endpoints
- Tests use `WebApplicationFactory` for in-memory testing
- All tests run automatically in CI/CD pipeline

### Adding a New Package

```bash
dotnet add package <PackageName>
```

## Configuration

Configuration is managed through `appsettings.json` and `appsettings.Development.json`.

Currently using default settings. Future configuration will include:
- Database connection strings
- API keys for external services
- Authentication settings
- Logging levels

## Contributing

Follow the project's [Conventional Commits](https://www.conventionalcommits.org/) guidelines when making changes.

All commits must follow the format:
```
<type>(<scope>): <description>
```

Example: `feat(api): add capture endpoint`

## License

MIT License - See LICENSE file for details
