using Microsoft.AspNetCore.Mvc;
using TrainBooking.Models.DTOs;
using TrainBooking.Services.Interfaces;

namespace TrainBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        /// <summary>
        /// Search available trains
        /// </summary>
        [HttpPost("trains")]
        public async Task<ActionResult<IEnumerable<TrainSearchResultDto>>> SearchTrains([FromBody] TrainSearchDto searchDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var results = await _searchService.SearchTrainsAsync(searchDto);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in train search");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Search available flights
        /// </summary>
        [HttpPost("flights")]
        public async Task<ActionResult<IEnumerable<FlightSearchResultDto>>> SearchFlights([FromBody] FlightSearchDto searchDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var results = await _searchService.SearchFlightsAsync(searchDto);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in flight search");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Get all stations
        /// </summary>
        [HttpGet("stations")]
        public async Task<ActionResult<IEnumerable<StationDto>>> GetStations()
        {
            try
            {
                var stations = await _searchService.GetAllStationsAsync();
                return Ok(stations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stations");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// /// Get all airports
        /// </summary>
        [HttpGet("airports")]
        public async Task<ActionResult<IEnumerable<AirportDto>>> GetAirports()
        {
            try
            {
                var airports = await _searchService.GetAllAirportsAsync();
                return Ok(airports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving airports");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Get lookup data (quota types, classes, payment modes, etc.)
        /// </summary>
        [HttpGet("lookup-data")]
        public async Task<ActionResult<LookupDataDto>> GetLookupData()
        {
            try
            {
                var lookupData = await _searchService.GetLookupDataAsync();
                return Ok(lookupData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lookup data");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }
    }
}