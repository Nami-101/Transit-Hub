using TrainBooking.Data;
using TrainBooking.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add HTTP Context Accessor for audit fields
builder.Services.AddHttpContextAccessor();

// Add application services
builder.Services.AddApplicationServices(builder.Configuration);

// Add Swagger documentation
builder.Services.AddSwaggerDocumentation();

// Add CORS
builder.Services.AddCorsPolicy();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments for development purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Train Booking API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Train Booking API Documentation";
    c.DefaultModelsExpandDepth(-1); // Hide models section by default
    c.DisplayRequestDuration();
});

// Use CORS
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");

// Use HTTPS redirection
app.UseHttpsRedirection();

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Seed data on startup (optional - you can also call the seed endpoint)
if (app.Environment.IsDevelopment())
{
    try
    {
        await app.SeedDataAsync();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// Add a simple root endpoint
app.MapGet("/", () => new
{
    Message = "Train Booking API is running",
    Documentation = "/swagger",
    Health = "/api/masterdata/health",
    Version = "1.0.0"
});

app.Run();