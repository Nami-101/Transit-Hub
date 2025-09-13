using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainBooking.Data;
using TrainBooking.Models.DTOs;

namespace TrainBooking.Controllers
{
    /// <summary>
    /// Controller for managing master data like stations and train classes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MasterDataController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MasterDataController> _logger;

        public MasterDataController(IServiceProvider serviceProvider, ILogger<MasterDataController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Seed master data (stations and train classes)
        /// </summary>
        /// <returns>Seeding result</returns>
        /// <response code="200">Data seeded successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("seed")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> SeedMasterData()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await dataSeeder.SeedAsync();

                _logger.LogInformation("Master data seeded successfully by user {UserId}",
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                return Ok(ApiResponse.SuccessResult("Master data seeded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding master data");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while seeding data"));
            }
        }        
/// <summary>
        /// Get system health status
        /// /// </summary>
        /// <returns>Health status</returns>
        /// <response code="200">System is healthy</response>
        [HttpGet("health")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public IActionResult GetHealth()
        {
            var healthInfo = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            };

            return Ok(ApiResponse<object>.SuccessResult(healthInfo, "System is healthy"));
        }

        /// <summary>
        /// Get all active stations
        /// </summary>
        /// <returns>List of stations</returns>
        /// <response code="200">Stations retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("stations")]
        [ProducesResponseType(typeof(ApiResponse<List<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetStations()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var stations = await context.Stations
                    .Where(s => s.IsActive)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Code,
                        s.City,
                        s.State,
                        s.Country
                    })
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                return Ok(ApiResponse<List<object>>.SuccessResult(stations.Cast<object>().ToList(), 
                    $"Retrieved {stations.Count} stations"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving stations");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving stations"));
            }
        }

        /// <summary>
        /// Get all active train classes
        /// </summary>
        /// <returns>List of train classes</returns>
        /// <response code="200">Train classes retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("trainclasses")]
        [ProducesResponseType(typeof(ApiResponse<List<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetTrainClasses()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var trainClasses = await context.TrainClasses
                    .Where(tc => tc.IsActive)
                    .Select(tc => new
                    {
                        tc.Id,
                        tc.Name,
                        tc.Code,
                        tc.Description,
                        tc.BaseFareMultiplier
                    })
                    .OrderBy(tc => tc.Name)
                    .ToListAsync();

                return Ok(ApiResponse<List<object>>.SuccessResult(trainClasses.Cast<object>().ToList(), 
                    $"Retrieved {trainClasses.Count} train classes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving train classes");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving train classes"));
            }
        }

        /// <summary>
        /// /// Get API information
        /// </summary>
        /// <returns>API information</returns>
        /// <response code="200">API information retrieved</response>
        [HttpGet("info")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public IActionResult GetApiInfo()
        {
            var apiInfo = new
            {
                Name = "Train Booking API",
                Version = "1.0.0",
                Description = "A comprehensive train booking system API similar to IRCTC",
                Documentation = "/swagger",
                Endpoints = new
                {
                    Authentication = "/api/auth",
                    MasterData = "/api/masterdata",
                    Health = "/api/masterdata/health"
                }
            };

            return Ok(ApiResponse<object>.SuccessResult(apiInfo, "API information retrieved"));
        }
    }
}