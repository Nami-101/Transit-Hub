using TransitHub.Models.DTOs;

namespace TransitHub.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto paymentDto);
        Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int? userId = null, int? bookingId = null, string? status = null);
        Task<PaymentResponseDto> ProcessRefundAsync(int bookingId, decimal refundAmount, decimal cancellationFee = 0, string? reason = null, string processedBy = "System");
    }
}