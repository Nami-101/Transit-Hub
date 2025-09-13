using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TrainBooking.Data
{
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(IServiceProvider serviceProvider, ILogger<DataSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Seed roles
                await SeedRolesAsync(roleManager);

                // Seed admin user
                await SeedAdminUserAsync(userManager);

                // Seed train classes
                await SeedTrainClassesAsync(context);

                // Seed stations
                await SeedStationsAsync(context);

                _logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding");
                throw;
            }
        }      
  private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Created role: {Role}", role);
                }
            }
        }

        private async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
        {
            const string adminEmail = "admin@trainbooking.com";
            const string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Created admin user: {Email}", adminEmail);
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}", 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        private async Task SeedTrainClassesAsync(ApplicationDbContext context)
        {
            try
            {
                // Check if TrainClasses table exists by trying to query it
                var tableExists = await CheckTableExistsAsync(context, "TrainClasses");
                
                if (tableExists)
                {
                    // Check if data already exists
                    var hasData = await HasDataInTableAsync(context, "TrainClasses");

                    if (!hasData)
                    {
                        var trainClasses = new[]
                        {
                            new { Name = "Sleeper", Code = "SL", Description = "Sleeper Class" },
                            new { Name = "Third AC", Code = "3A", Description = "Third AC Class" },
                            new { Name = "Second AC", Code = "2A", Description = "Second AC Class" },
                            new { Name = "First AC", Code = "1A", Description = "First AC Class" }
                        };

                        foreach (var trainClass in trainClasses)
                        {
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO TrainClasses (Name, Code, Description, IsActive, CreatedAt, UpdatedAt) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                                trainClass.Name, trainClass.Code, trainClass.Description, true, DateTime.UtcNow, DateTime.UtcNow);
                        }

                        _logger.LogInformation("Seeded train classes");
                    }
                }
                else
                {
                    _logger.LogWarning("TrainClasses table does not exist yet. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not seed train classes. Table may not exist yet.");
            }
        }

        private async Task SeedStationsAsync(ApplicationDbContext context)
        {
            try
            {
                // Check if Stations table exists by trying to query it
                var tableExists = await CheckTableExistsAsync(context, "Stations");
                
                if (tableExists)
                {
                    // Check if data already exists
                    var hasData = await HasDataInTableAsync(context, "Stations");

                    if (!hasData)
                    {
                        var stations = new[]
                        {
                            new { Name = "Chennai Central", Code = "MAS", City = "Chennai", State = "Tamil Nadu" },
                            new { Name = "Bangalore City", Code = "SBC", City = "Bangalore", State = "Karnataka" },
                            new { Name = "New Delhi", Code = "NDLS", City = "Delhi", State = "Delhi" }
                        };

                        foreach (var station in stations)
                        {
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO Stations (Name, Code, City, State, IsActive, CreatedAt, UpdatedAt) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                                station.Name, station.Code, station.City, station.State, true, DateTime.UtcNow, DateTime.UtcNow);
                        }

                        _logger.LogInformation("Seeded stations");
                    }
                }
                else
                {
                    _logger.LogWarning("Stations table does not exist yet. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not seed stations. Table may not exist yet.");
            }
        }

        private async Task<bool> CheckTableExistsAsync(ApplicationDbContext context, string tableName)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}", tableName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> HasDataInTableAsync(ApplicationDbContext context, string tableName)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    $"SELECT TOP 1 1 FROM {tableName} WHERE IsActive = 1");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    // Extension method to easily call seeding
    public static class DataSeederExtensions
    {
        public static async Task<IHost> SeedDataAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedAsync();
            return host;
        }
    }
}