using Microsoft.EntityFrameworkCore;
using TransitHub.Data;
using TransitHub.Repositories;
using TransitHub.Repositories.Interfaces;
using TransitHub.Services;
using TransitHub.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Entity Framework
builder.Services.AddDbContext<TransitHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add Business Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Transit-Hub API", 
        Version = "v1",
        Description = "API for Transit-Hub booking system with Generic Repository + Unit of Work pattern"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transit-Hub API V1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
});

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TransitHubDbContext>();
    try
    {
        // Apply any pending migrations
        context.Database.Migrate();
        
        // Create stored procedures
        await CreateStoredProceduresAsync(context);
        
        // Seed initial data
        DataSeeder.SeedAsync(context).Wait();
        
        Console.WriteLine("Database setup and seeding completed successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database setup failed: {ex.Message}");
    }
}

static async Task CreateStoredProceduresAsync(TransitHubDbContext context)
{
    try
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "StoredProcedures");
        
        // Read and execute stored procedure files in order
        var files = new[]
        {
            "01_UserManagement.sql",
            "02_SearchOperations.sql", 
            "03_BookingOperations.sql",
            "04_PaymentOperations.sql",
            "05_WaitlistOperations.sql",
            "06_AdminOperations.sql"
        };
        
        foreach (var file in files)
        {
            var filePath = Path.Combine(basePath, file);
            if (File.Exists(filePath))
            {
                var sql = await File.ReadAllTextAsync(filePath);
                // Split by GO statements and execute each batch
                var batches = sql.Split(new[] { "\nGO\n", "\nGO\r\n", "\rGO\r", "\nGO", "GO\n" }, 
                    StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var batch in batches)
                {
                    var trimmedBatch = batch.Trim();
                    if (!string.IsNullOrEmpty(trimmedBatch) && !trimmedBatch.StartsWith("--") && !trimmedBatch.StartsWith("USE"))
                    {
                        try
                        {
                            await context.Database.ExecuteSqlRawAsync(trimmedBatch);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Warning: Failed to execute batch from {file}: {ex.Message}");
                        }
                    }
                }
                Console.WriteLine($"Executed stored procedures from {file}");
            }
        }
        
        Console.WriteLine("All stored procedures created successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating stored procedures: {ex.Message}");
    }
}

app.Run();