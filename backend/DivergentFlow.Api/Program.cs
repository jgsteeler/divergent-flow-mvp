using DivergentFlow.Api.Extensions;
using DivergentFlow.Api.Middleware;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.DependencyInjection;
using DivergentFlow.Infrastructure.DependencyInjection;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Local dev convenience: load env vars from a .env file (if present).
// Notes:
// - Uses dotenv.net package for .env file loading
// - Starts searching from ContentRootPath/.env and searches up to 4 parent directories
// - Only sets variables that aren't already set in the process environment
// - Ignores missing .env files (doesn't throw exceptions)
DotEnv.Load(new DotEnvOptions(
    // Prefer a service-local .env, but also support backend/.env (common in this repo).
    envFilePaths:
    [
        Path.Combine(builder.Environment.ContentRootPath, ".env"),
        Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"))
    ],
    ignoreExceptions: true,
    overwriteExistingVars: false,
    probeLevelsToSearch: 4
));

// Important: the EnvironmentVariables configuration provider loads once at startup.
// Since dotenv.net sets environment variables at runtime, reload configuration so
// builder.Configuration can see values from backend/.env (e.g. UPSTASH_REDIS_REST_URL).
if (builder.Configuration is IConfigurationRoot configurationRoot)
{
    configurationRoot.Reload();
}

// Add services to the container
builder.Services.AddControllers();

// Add CORS policy (environment-driven)
builder.Services.AddCorsPolicy(builder.Environment);

// Register services for dependency injection using extension method
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Divergent Flow API",
        Version = "v1",
        Description = "ADHD-friendly brain management tool API"
    });
});

var app = builder.Build();

// One-time startup diagnostic: log which repositories are active.
using (var scope = app.Services.CreateScope())
{
    var captureRepository = scope.ServiceProvider.GetRequiredService<ICaptureRepository>();
    var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
    
    app.Logger.LogInformation(
        "Active ICaptureRepository implementation: {RepositoryType}",
        captureRepository.GetType().FullName);
    
    app.Logger.LogInformation(
        "Active IItemRepository implementation: {RepositoryType}",
        itemRepository.GetType().FullName);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Divergent Flow API v1");
    });
}

// Enable CORS
app.UseCors();

// Map FluentValidation (MediatR pipeline) failures to HTTP 400
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapControllers();

app.Run();

// Make Program class accessible to tests
public partial class Program { }
