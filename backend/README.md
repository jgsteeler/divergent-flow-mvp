# Divergent Flow API

ADHD-friendly brain management tool - Backend Web API

## Overview

This is a .NET 10 Web API that provides backend services for the Divergent Flow application. The API provides CRUD operations for managing capture items.

## Features

- ✅ RESTful API endpoints for capture and item management
- ✅ MongoDB persistence as system of record
- ✅ Background type inference queue with asynchronous processing
- ✅ Redis projection cache for eventual consistency (optional, via Upstash REST)
- ✅ Swagger/OpenAPI documentation
- ✅ CORS enabled for frontend integration
- ✅ Dependency injection architecture
- ✅ Repository pattern for data persistence
- ✅ Zero authentication (will be added in future phases)

## Tech Stack

- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core Web API** - Web API framework
- **MongoDB.Driver** - MongoDB client for data persistence
- **Upstash Redis (REST)** - Serverless Redis hosting (optional)
- **Swashbuckle** - Swagger/OpenAPI documentation
- **xUnit** - Testing framework
- **Moq** - Mocking framework for tests
- **C# 13** - Programming language

## Getting Started

### Prerequisites

- .NET 10 SDK or later
- Any IDE (Visual Studio, VS Code, Rider, etc.)
- MongoDB (local or Atlas) - **Required**
- Upstash Redis (REST) - Optional, for projection cache

### Setting Up MongoDB (Required)

This API uses MongoDB as the primary data store (system of record).

**See [MONGODB-SETUP.md](MONGODB-SETUP.md) for complete MongoDB setup instructions.**

Quick start for local development:

```bash
# macOS
brew tap mongodb/brew
brew install mongodb-community@8.0
brew services start mongodb-community@8.0

# Ubuntu/Debian
# See MONGODB-SETUP.md for full instructions

# Or use Docker
docker run -d -p 27017:27017 --name mongodb mongo:8
```

Then set environment variables in `.env`:

```bash
MONGODB_CONNECTION_STRING=mongodb://localhost:27017
MONGODB_DATABASE_NAME=divergent_flow
```

### Setting Up Redis (Optional)

Redis is used as a projection cache for eventual consistency. The API will work without Redis.

This API supports Redis via Upstash REST (one connectivity mode across environments).

#### Using Upstash Redis (REST)

1. Create a free account at [Upstash](https://upstash.com/)
2. Create a new Redis database
3. Copy the connection details:

- **REST URL**: HTTPS REST URL from the Upstash console (e.g., `https://us1-merry-cat-32748.upstash.io`)
- **REST Token**: Upstash REST token (Bearer)

4. Set environment variables (see Configuration section below)

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

### Items (Unified Domain Model)

Items are the foundational unit of user-created content. "Capture" is the default item type.

#### GET /api/items

Get all items

- **Response**: `200 OK` with array of `ItemDto`

#### GET /api/items/{id}

Get a specific item by ID

- **Parameters**: `id` (string) - Item ID
- **Response**: `200 OK` with `ItemDto` or `404 Not Found`

#### POST /api/items

Create a new item (automatically enqueued for background type inference)

- **Request Body**: `CreateItemRequest`

  ```json
  {
    "text": "Your item text here",
    "inferredType": null,
    "typeConfidence": null,
    "collectionId": null
  }
  ```

- **Response**: `201 Created` with `ItemDto`

#### PUT /api/items/{id}

Update an existing item (re-enqueued for inference if text changes)

- **Parameters**: `id` (string) - Item ID
- **Request Body**: `UpdateItemRequest`

  ```json
  {
    "text": "Updated item text",
    "inferredType": "note",
    "typeConfidence": 85,
    "collectionId": null
  }
  ```

- **Response**: `200 OK` with `ItemDto` or `404 Not Found`

#### DELETE /api/items/{id}

Delete an item

- **Parameters**: `id` (string) - Item ID
- **Response**: `204 No Content` or `404 Not Found`

### Captures (Legacy Compatibility)

The `/api/captures` endpoints remain for backward compatibility and map to the same underlying Item entities.

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

### ItemDto

```csharp
{
  "id": "string",                // UUID
  "type": "capture",             // Item type (default: "capture")
  "text": "string",              // Item content
  "createdAt": 1234567890,       // Unix timestamp in milliseconds
  "inferredType": "note",        // Inferred type (e.g., "note", "action", "reminder")
  "typeConfidence": 85.5,        // Confidence level (0-100)
  "lastReviewedAt": 1234567890,  // Unix timestamp (null if not reviewed)
  "collectionId": "string"       // Collection ID (null if not in a collection)
}
```

### CaptureDto (Legacy)

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
│   │   ├── CapturesController.cs    # Legacy capture endpoints
│   │   └── ItemsController.cs       # Item endpoints (primary)
│   └── Program.cs                    # Application startup
├── DivergentFlow.Domain/             # Core domain entities (Item, Collection)
├── DivergentFlow.Application/        # Use-cases, handlers, abstractions, queue
├── DivergentFlow.Infrastructure/     # MongoDB, Redis implementations
├── DivergentFlow.Api.Tests/          # API tests
├── DivergentFlow.Application.Tests/  # Application-layer unit tests
├── DivergentFlow.Infrastructure.Tests/ # Infrastructure-layer unit tests
├── DivergentFlow.sln                 # Solution file
├── MONGODB-SETUP.md                  # MongoDB setup guide
└── version.txt                       # Version tracking for releases
```

## Architecture

The API follows a CQRS-inspired layered/clean architecture pattern:

1. **Api** - HTTP controllers
2. **Application** - Use-cases (MediatR handlers), validation, abstractions, background processing
3. **Domain** - Core entities (Item, Collection)
4. **Infrastructure** - Concrete integrations (MongoDB, Redis)

### Data Flow

#### Write Path (Command)

```
Controller → MediatR Handler → MongoDB (system of record)
                              ↓
                    Enqueue for background inference
                              ↓
                    Background Worker → Type Inference
                              ↓
                    Update MongoDB → Sync to Redis (projection cache)
```

#### Read Path (Query)

```
Controller → MediatR Handler → MongoDB (direct read)
```

- **Controllers** depend on **MediatR**
- **Handlers** depend on **abstractions** (e.g. `IItemRepository`, `IInferenceQueue`)
- **Infrastructure** provides concrete implementations (MongoDB, Redis)

This allows for:

- Easy swapping of implementations (e.g., MongoDB → PostgreSQL)
- Comprehensive unit testing with mocks
- Clear separation of concerns
- Asynchronous processing with eventual consistency

## CORS Configuration

CORS is configured to allow requests from the frontend:

- `http://localhost:5173` (Vite dev server)
- `http://localhost:5000` (Alternative port)

## Background Type Inference Workflow

The API includes two background services for type inference:

### 1. InferenceQueueProcessorService (Primary - New)

Processes items from an in-process queue asynchronously:

1. When an item is created via `POST /api/items`, it's immediately enqueued
2. When an item is updated and text changes, it's re-enqueued
3. The background worker dequeues items one at a time
4. For each item:
   - Loads the item from MongoDB
   - Runs type inference
   - Updates the item in MongoDB if confidence improves
   - Syncs the updated item to Redis projection cache (if available)

### 2. BackgroundTypeInferenceService (Periodic - Existing)

Runs periodically to catch any items that weren't processed:

1. Runs every 60 seconds (configurable)
2. Queries for non-migrated captures with:
   - `null` TypeConfidence, OR
   - TypeConfidence < configured threshold (default: 95)
3. For each eligible capture, runs type inference
4. Updates if new confidence is higher than existing

### Configuration

Configure in `appsettings.json` or via environment variables:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "divergent_flow",
    "ItemsCollectionName": "items",
    "CollectionsCollectionName": "collections"
  },
  "TypeInference": {
    "ConfidenceThreshold": 95,
    "ProcessingIntervalSeconds": 60
  }
}
```

Or via environment variables:

```bash
# MongoDB
MongoDB__ConnectionString=mongodb://localhost:27017
MongoDB__DatabaseName=divergent_flow

# Type Inference
TypeInference__ConfidenceThreshold=95
TypeInference__ProcessingIntervalSeconds=60
```

### Future Enhancements

Currently, the basic inference service returns fixed values (MVP). As learning models are built from user confirmations in the review queue, this background workflow will automatically pick up the improved inference results.

## Future Enhancements

- [ ] Authentication & Authorization
- [x] MongoDB persistence as system of record
- [x] Background type inference queue with asynchronous processing
- [x] Redis projection cache for eventual consistency
- [ ] Property validation endpoints
- [ ] LLM integration for intelligent processing
- [ ] Rate limiting
- [ ] Enhanced logging and monitoring
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

### MongoDB (Required)

**See [MONGODB-SETUP.md](MONGODB-SETUP.md) for complete setup instructions.**

Required environment variables:

- `MONGODB_CONNECTION_STRING`: MongoDB connection string
- `MONGODB_DATABASE_NAME`: Database name

Optional environment variables:

- `MONGODB_ITEMS_COLLECTION`: Items collection name (default: "items")
- `MONGODB_COLLECTIONS_COLLECTION`: Collections collection name (default: "collections")

Or use the double-underscore notation:

```bash
MongoDB__ConnectionString=mongodb://localhost:27017
MongoDB__DatabaseName=divergent_flow
```

### Redis (Optional - Projection Cache)

This project supports a single Redis connectivity mode:

- **Upstash Redis (REST API via HttpClient)**

#### Upstash (REST) configuration

- `UPSTASH_REDIS_REST_URL`: HTTPS REST URL from the Upstash console (e.g. `https://us1-merry-cat-32748.upstash.io`)
- `UPSTASH_REDIS_REST_TOKEN`: Upstash REST token (Bearer)
- `UPSTASH_REDIS_REST_READONLY_TOKEN`: optional readonly token

**Important Notes:**

- **MongoDB is required** - The API will fail to start without MongoDB configuration
- **Redis is optional** - The API will work without Redis; a no-op projection writer is used
- For Upstash REST: copy the **HTTPS REST URL** + **REST token** from the Upstash console

## Contributing

Follow the project's [Conventional Commits](https://www.conventionalcommits.org/) guidelines when making changes.

All commits must follow the format:

```
<type>(<scope>): <description>
```

Example: `feat(api): add capture endpoint`

## License

MIT License - See LICENSE file for details
