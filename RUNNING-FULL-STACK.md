# Running the Full Stack Application

This guide explains how to run both the frontend and backend together.

## Prerequisites

- Node.js (for frontend)
- .NET 9 SDK (for backend)

## Quick Start

### Terminal 1: Start the Backend API

```bash
cd api/DivergentFlow.Api
dotnet run
```

The API will be available at `http://localhost:5100`

**Swagger UI**: `http://localhost:5100/swagger`

### Terminal 2: Start the Frontend

```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

## Verification

Once both are running:

1. **Test Backend**: Visit `http://localhost:5100/swagger` to see the API documentation
2. **Test Frontend**: Visit `http://localhost:5173` to use the application
3. **Test Integration**: Create a capture in the frontend and verify it's sent to the backend

## Default Ports

- **Frontend (Vite)**: `http://localhost:5173`
- **Backend (API)**: `http://localhost:5100`

These ports are configured in the CORS settings of the API.

## Troubleshooting

### Port Already in Use

If you get a port conflict:

**Frontend:**
```bash
# Vite will automatically try the next available port
npm run dev
```

**Backend:**
```bash
# Specify a different port
dotnet run --urls "http://localhost:5101"
```

Remember to update CORS settings in `api/DivergentFlow.Api/Program.cs` if you change ports.

### CORS Errors

If you see CORS errors in the browser console:

1. Check that the backend is running on `http://localhost:5100`
2. Check that the frontend is running on `http://localhost:5173` or `http://localhost:5000`
3. If using different ports, update the CORS configuration in `Program.cs`:

```csharp
policy.WithOrigins("http://localhost:YOUR_PORT")
```

### Connection Refused

If the frontend can't connect to the backend:

1. Verify the backend is running: `curl http://localhost:5100/api/captures`
2. Check for firewall issues
3. Verify the API URL in the frontend code matches the backend port

## Development Workflow

1. Start both servers
2. Make changes to code
3. Frontend auto-reloads (Vite HMR)
4. Backend requires manual restart (or use `dotnet watch` for auto-reload)

### Using dotnet watch (Recommended)

For automatic backend reloading on code changes:

```bash
cd api/DivergentFlow.Api
dotnet watch
```

## Next Steps

- Frontend integration with the API (replace Spark KV with API calls)
- Add type inference endpoints
- Add property validation endpoints
- Implement database storage
- Add authentication
