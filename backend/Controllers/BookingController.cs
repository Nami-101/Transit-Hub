using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitHub.Models.DTOs;
using TransitHub.Services.Interfaces;

namespace TransitHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all booking operations
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Create a train booking
        /// </summary>
        [HttpPost("train")]
        public async Task<ActionResult<BookingResponseDto>> CreateTrainBooking([FromBody] CreateTrainBookingDto bookingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _bookingService.CreateTrainBookingAsync(bookingDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in train booking creation");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Create a flight booking
        /// </summary>
        [HttpPost("flight")]
        public async Task<ActionResult<BookingResponseDto>> CreateFlightBooking([FromBody] CreateFlightBookingDto bookingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _bookingService.CreateFlightBookingAsync(bookingDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in flight booking creation");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Get user bookings with optional filters
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserBookingDto>>> GetUserBookings(
            int userId, 
            [FromQuery] string? bookingType = null, 
            [FromQuery] string? status = null)
        {
            try
            {
                // Get current user ID from JWT token
                var currentUserId = User?.FindFirst("sub")?.Value ?? User?.FindFirst("id")?.Value;
                
                // Ensure user can only access their own bookings
                if (currentUserId != userId.ToString())
                {
                    return Forbid("You can only access your own bookings");
                }
                
                var bookings = await _bookingService.GetUserBookingsAsync(userId, bookingType, status);
                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Get detailed booking information
        /// </summary>
        [HttpGet("{bookingId}/user/{userId}")]
        public async Task<ActionResult<BookingDetailsDto>> GetBookingDetails(int bookingId, int userId)
        {
            try
            {
                // Get current user ID from JWT token
                var currentUserId = User?.FindFirst("sub")?.Value ?? User?.FindFirst("id")?.Value;
                
                // Ensure user can only access their own bookings
                if (currentUserId != userId.ToString())
                {
                    return Forbid("You can only access your own bookings");
                }
                
                var bookingDetails = await _bookingService.GetBookingDetailsAsync(bookingId, userId);
                return Ok(bookingDetails);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking details for booking {BookingId}", bookingId);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        [HttpPost("{bookingId}/cancel")]
        public async Task<ActionResult<CancellationResponseDto>> CancelBooking(
            int bookingId, 
            [FromBody] CancelBookingRequestDto cancelRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get current user ID from JWT token
                var currentUserId = User?.FindFirst("sub")?.Value ?? User?.FindFirst("id")?.Value;
                
                // Ensure user can only cancel their own bookings
                if (currentUserId != cancelRequest.UserId.ToString())
                {
                    return Forbid("You can only cancel your own bookings");
                }

                var result = await _bookingService.CancelBookingAsync(bookingId, cancelRequest.UserId, cancelRequest.Reason);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }
    }

    // Helper DTO for cancellation request
    public class CancelBookingRequestDto
    {
        public int UserId { get; set; }
        public string? Reason { get; set; }
    }
}