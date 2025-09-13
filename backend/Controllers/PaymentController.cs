using Microsoft.AspNetCore.Mvc;
using TransitHub.Models.DTOs;
using TransitHub.Services.Interfaces;

namespace TransitHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Process payment for a booking
        /// </summary>
        [HttpPost("process")]
        public async Task<ActionResult<PaymentResponseDto>> ProcessPayment([FromBody] ProcessPaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _paymentService.ProcessPaymentAsync(paymentDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for booking {BookingId}", paymentDto.BookingID);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Get payment history for a user or booking
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentHistory(
            [FromQuery] int? userId = null,
            [FromQuery] int? bookingId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                // Validate that at least one parameter is provided
                if (userId == null && bookingId == null)
                {
                    return BadRequest(new { Message = "Either userId or bookingId must be provided" });
                }

                // TODO: Add JWT authentication and ensure user can only access their own payment history
                var paymentHistory = await _paymentService.GetPaymentHistoryAsync(userId, bookingId, status);
                return Ok(paymentHistory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Process refund for a cancelled booking
        /// </summary>
        [HttpPost("refund")]
        public async Task<ActionResult<PaymentResponseDto>> ProcessRefund([FromBody] ProcessRefundDto refundDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // TODO: Add authorization - only admins should be able to process refunds
                var result = await _paymentService.ProcessRefundAsync(
                    refundDto.BookingId, 
                    refundDto.RefundAmount, 
                    refundDto.CancellationFee, 
                    refundDto.Reason, 
                    refundDto.ProcessedBy ?? "Admin");
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for booking {BookingId}", refundDto.BookingId);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }
    }

    // Helper DTO for refund request
    public class ProcessRefundDto
    {
        public int BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationFee { get; set; } = 0;
        public string? Reason { get; set; }
        public string? ProcessedBy { get; set; }
    }
}