using Microsoft.Data.SqlClient;
using TransitHub.Models.DTOs;
using TransitHub.Repositories.Interfaces;
using TransitHub.Services.Interfaces;

namespace TransitHub.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto paymentDto)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@BookingID", paymentDto.BookingID),
                    new SqlParameter("@PaymentModeID", paymentDto.PaymentModeID),
                    new SqlParameter("@Amount", paymentDto.Amount),
                    new SqlParameter("@TransactionRef", (object?)paymentDto.TransactionRef ?? DBNull.Value),
                    new SqlParameter("@CreatedBy", "PaymentService")
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<PaymentResponseDto>(
                    "sp_ProcessPayment", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Payment processing failed. Please try again."
                    };
                }

                _logger.LogInformation("Payment processed: {TransactionRef}, Status: {Status}", 
                    response.TransactionReference, response.Status);
                return response;
            }
            catch (SqlException ex) when (ex.Number >= 50023 && ex.Number <= 50027)
            {
                _logger.LogWarning("Payment validation error: {Message}", ex.Message);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for booking {BookingId}", paymentDto.BookingID);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Payment processing failed due to a system error. Please try again later."
                };
            }
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int? userId = null, int? bookingId = null, string? status = null)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@UserID", (object?)userId ?? DBNull.Value),
                    new SqlParameter("@BookingID", (object?)bookingId ?? DBNull.Value),
                    new SqlParameter("@Status", (object?)status ?? DBNull.Value)
                };

                // Execute stored procedure - this will return a different structure than PaymentDto
                // We need to create a custom DTO for the stored procedure result
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<PaymentHistoryResultDto>(
                    "sp_GetPaymentHistory", parameters);

                // Convert to PaymentDto
                var paymentHistory = result.Select(p => new PaymentDto
                {
                    PaymentID = p.PaymentID,
                    Amount = p.Amount,
                    Status = p.Status,
                    TransactionRef = p.TransactionRef,
                    PaymentDate = p.PaymentDate,
                    PaymentMode = p.PaymentMode
                });

                _logger.LogInformation("Retrieved {Count} payment records", paymentHistory.Count());
                return paymentHistory;
            }
            catch (SqlException ex) when (ex.Number == 50028)
            {
                _logger.LogWarning("Payment history validation error: {Message}", ex.Message);
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history");
                throw;
            }
        }

        public async Task<PaymentResponseDto> ProcessRefundAsync(int bookingId, decimal refundAmount, decimal cancellationFee = 0, string? reason = null, string processedBy = "System")
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@BookingID", bookingId),
                    new SqlParameter("@RefundAmount", refundAmount),
                    new SqlParameter("@CancellationFee", cancellationFee),
                    new SqlParameter("@CancellationReason", (object?)reason ?? DBNull.Value),
                    new SqlParameter("@ProcessedBy", processedBy)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<RefundResponseDto>(
                    "sp_ProcessRefund", parameters);

                var refundResult = result.FirstOrDefault();
                if (refundResult == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Refund processing failed. Please try again."
                    };
                }

                // Convert RefundResponseDto to PaymentResponseDto
                var response = new PaymentResponseDto
                {
                    PaymentID = refundResult.RefundPaymentID,
                    Status = "Refunded",
                    TransactionReference = $"REF_{bookingId}_{DateTime.Now:yyyyMMddHHmmss}",
                    Amount = refundResult.RefundAmount,
                    Message = refundResult.Message,
                    Success = refundResult.Success
                };

                _logger.LogInformation("Refund processed for booking {BookingId}: Amount {Amount}", 
                    bookingId, refundAmount);
                return response;
            }
            catch (SqlException ex) when (ex.Number >= 50029 && ex.Number <= 50031)
            {
                _logger.LogWarning("Refund validation error: {Message}", ex.Message);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for booking {BookingId}", bookingId);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Refund processing failed due to a system error. Please try again later."
                };
            }
        }
    }

    // Helper DTOs for stored procedure results
    public class PaymentHistoryResultDto
    {
        public int PaymentID { get; set; }
        public int BookingID { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string BookingType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TransactionRef { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }

    public class RefundResponseDto
    {
        public int RefundPaymentID { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationFee { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}