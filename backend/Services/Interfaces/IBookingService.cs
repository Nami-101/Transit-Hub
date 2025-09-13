using TrainBooking.Models.DTOs;

namespace TrainBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateTrainBookingAsync(CreateTrainBookingDto bookingDto);
        Task<BookingResponseDto> CreateFlightBookingAsync(CreateFlightBookingDto bookingDto);
        Task<IEnumerable<UserBookingDto>> GetUserBookingsAsync(int userId, string? bookingType = null, string? status = null);
        Task<BookingDetailsDto> GetBookingDetailsAsync(int bookingId, int userId);
        Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int userId, string? reason = null);
    }
}