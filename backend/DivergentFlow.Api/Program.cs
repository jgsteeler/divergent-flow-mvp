using DivergentFlow.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CORS to allow frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
              // Allow any localhost/127.0.0.1 origin so Vite can pick any port.
              .SetIsOriginAllowed(origin =>
              {
                  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                  {
                      return false;
                  }

                  return uri.Scheme is "http" or "https" &&
                         (uri.Host == "localhost" || uri.Host == "127.0.0.1");
              })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register services for dependency injection using extension method
builder.Services.UseServices();

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
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapGet("/hello", () => Results.Ok("hello"));

app.MapControllers();

app.Run();

// Make Program class accessible to tests
public partial class Program { }
