# Divergent Flow API

ADHD-friendly brain management tool - Backend Web API

## Overview

This is a .NET 10 Web API that provides backend services for the Divergent Flow application. The API provides CRUD operations for managing capture items.

## Features

- ✅ RESTful API endpoints for capture management
- ✅ Swagger/OpenAPI documentation
- ✅ CORS enabled for frontend integration
- ✅ Dependency injection architecture
- ✅ Redis data storage with Upstash integration
- ✅ Repository pattern for data persistence
- ✅ Zero authentication (will be added in future phases)

## Tech Stack

- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core Web API** - Web API framework
- **StackExchange.Redis** - Redis client
- **Upstash Redis** - Serverless Redis hosting
- **Swashbuckle** - Swagger/OpenAPI documentation
- **xUnit** - Testing framework
- **Moq** - Mocking framework for tests
- **C# 13** - Programming language

## Getting Started

### Prerequisites

- .NET 10 SDK or later
- Any IDE (Visual Studio, VS Code, Rider, etc.)
- Upstash Redis account (for production) or local Redis (for development)

### Setting Up Redis

This API uses Redis for data persistence via Upstash (serverless Redis hosting).

#### Option 1: Using Upstash Redis (Recommended for Production)

1. Create a free account at [Upstash](https://upstash.com/)
2. Create a new Redis database
3. Copy the connection details:
   - **Redis URL**: The host:port (e.g., `fly-div-flo-staging.upstash.io:6379`)
   - **Redis Token**: The password/token from your Upstash dashboard
4. Set environment variables (see Configuration section below)

#### Option 2: Using Local Redis (Development)

1. Install Redis locally:

   ```bash
   # macOS
   brew install redis
   brew services start redis
   
   # Ubuntu/Debian
   sudo apt install redis-server
   sudo systemctl start redis
   
   # Windows (use WSL or Docker)
   docker run -d -p 6379:6379 redis
   ```

2. Set environment variables:
   - `REDIS_URL=localhost:6379`
   - `REDIS_TOKEN=` (leave empty for local Redis with no auth)

### Running the API

```bash
cd backend/DivergentFlow.Api
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
backend/
├── DivergentFlow.Api/
│   ├── Controllers/
│   │   └── CapturesController.cs    # API endpoints
│   └── Program.cs                    # Application startup
├── DivergentFlow.Domain/             # Core domain entities
├── DivergentFlow.Application/        # Use-cases, handlers, abstractions
├── DivergentFlow.Infrastructure/     # Redis + external services implementations
├── DivergentFlow.Api.Tests/
│   └── CapturesControllerTests.cs   # API tests
├── DivergentFlow.Application.Tests/  # Application-layer unit tests
├── DivergentFlow.Infrastructure.Tests/ # Infrastructure-layer unit tests
├── DivergentFlow.sln                 # Solution file
└── version.txt                       # Version tracking for releases
```

## Architecture

The API follows a layered/clean architecture pattern:

1. **Api** - HTTP controllers
2. **Application** - Use-cases (MediatR handlers), validation, abstractions
3. **Domain** - Core entities
4. **Infrastructure** - Concrete integrations (Redis, etc.)

### Layered Architecture

```
Controller → MediatR Handler → Repository → Redis/Storage
```

- **Controllers** depend on **MediatR**
- **Handlers** depend on **abstractions** (e.g. `ICaptureRepository`)
- **Infrastructure** provides concrete implementations (Redis)

This allows for easy swapping of implementations and comprehensive testing.

### Dependency Injection

Services are registered via extension methods:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
```

This allows for:

- Easy swapping of implementations (e.g., Redis → PostgreSQL)
- Comprehensive unit testing with mocks
- Clear separation of concerns

## CORS Configuration

CORS is configured to allow requests from the frontend:

- `http://localhost:5173` (Vite dev server)
- `http://localhost:5000` (Alternative port)

## Future Enhancements

- [ ] Authentication & Authorization
- [ ] Type inference endpoints
- [ ] Property validation endpoints
- [ ] LLM integration for intelligent processing
- [ ] Rate limiting
- [ ] Enhanced logging and monitoring
- [ ] Redis connection pooling optimization
- [ ] Data migration utilities

## Development

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

Tests are located in:

- `DivergentFlow.Api.Tests` - API/controller tests
- `DivergentFlow.Application.Tests` - Application-layer unit tests
- `DivergentFlow.Infrastructure.Tests` - Infrastructure-layer unit tests

All tests run automatically in CI/CD pipeline.

### Adding a New Package

```bash
dotnet add package <PackageName>
```

## Configuration

Configuration is managed through environment variables.

For local development:

- Copy `.env.example` to `.env` in the `backend` folder.
- Adjust the values as needed for your local environment; the `.env` file is gitignored and is loaded automatically by the API at startup.

### CORS

The `.env.example` file includes the following CORS-related variable with a dummy value:

- `CORS_ALLOWED_ORIGINS`: comma-separated list of allowed origins (e.g. `http://localhost:5173,https://app.getdivergentflow.com`)

Notes:

- In `Development`, the API also allows `localhost`/`127.0.0.1` on any port to keep local Vite workflows frictionless.
- In `Staging` and `Production`, the API fails closed if `CORS_ALLOWED_ORIGINS` is not set.

### Redis

This project supports two Redis connectivity modes:

- **Local Docker Redis (TCP via StackExchange.Redis)**
- **Upstash Redis (REST API via HttpClient)**

#### Upstash (REST) configuration (recommended for Upstash)

- `UPSTASH_REDIS_REST_URL`: HTTPS REST URL from the Upstash console (e.g. `https://us1-merry-cat-32748.upstash.io`)
- `UPSTASH_REDIS_REST_TOKEN`: Upstash REST token (Bearer)
- `UPSTASH_REDIS_REST_READONLY_TOKEN`: optional readonly token

Back-compat:

- `REDIS_URL` + `REDIS_TOKEN` can also be used for Upstash REST if you already have those wired as secrets.

#### Local Docker Redis (TCP) configuration

The `.env.example` file includes these TCP-related variables:

- `REDIS_URL`: Redis endpoint in `host:port` format (e.g. `redis:6379` in docker-compose or `localhost:6379` when running the API directly)
- `REDIS_TOKEN`: Redis password/token (optional for local Redis without auth)
- Optional: `REDIS_SSL`: `true`/`false` override (helpful for hosted Redis)

Alternative configuration:

- `REDIS_CONNECTION_STRING` (StackExchange.Redis format)
- `ConnectionStrings:Redis` (appsettings / environment)
- `Redis:ConnectionString` (appsettings / environment)

**Important Notes:**

- The API will throw an exception at startup if no Redis configuration is provided
- For Upstash REST: copy the **HTTPS REST URL** + **REST token** from the Upstash console.
- For local Redis: use `localhost:6379` and leave token empty if no auth is configured.

## Contributing

Follow the project's [Conventional Commits](https://www.conventionalcommits.org/) guidelines when making changes.

All commits must follow the format:

```
<type>(<scope>): <description>
```

Example: `feat(api): add capture endpoint`

## License

MIT License - See LICENSE file for details
