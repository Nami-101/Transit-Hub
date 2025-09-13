using Microsoft.Data.SqlClient;
using System.Text.Json;
using TransitHub.Models.DTOs;
using TransitHub.Repositories.Interfaces;
using TransitHub.Services.Interfaces;

namespace TransitHub.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BookingResponseDto> CreateTrainBookingAsync(CreateTrainBookingDto bookingDto)
        {
            try
            {
                // Convert passengers to JSON format for stored procedure
                var passengersJson = JsonSerializer.Serialize(bookingDto.Passengers.Select(p => new
                {
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender
                }));

                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@UserID", bookingDto.UserID),
                    new SqlParameter("@TrainScheduleID", bookingDto.TrainScheduleID),
                    new SqlParameter("@PassengerDetails", passengersJson),
                    new SqlParameter("@CreatedBy", "BookingService")
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<BookingResponseDto>(
                    "sp_CreateTrainBooking", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new BookingResponseDto
                    {
                        Success = false,
                        Message = "Train booking creation failed. Please try again."
                    };
                }

                _logger.LogInformation("Train booking created successfully: {BookingReference}", response.BookingReference);
                return response;
            }
            catch (SqlException ex) when (ex.Number >= 50016 && ex.Number <= 50022)
            {
                _logger.LogWarning("Train booking validation error: {Message}", ex.Message);
                return new BookingResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating train booking for user {UserId}", bookingDto.UserID);
                return new BookingResponseDto
                {
                    Success = false,
                    Message = "Booking creation failed due to a system error. Please try again later."
                };
            }
        }

        public async Task<BookingResponseDto> CreateFlightBookingAsync(CreateFlightBookingDto bookingDto)
        {
            try
            {
                // Convert passengers to JSON format for stored procedure
                var passengersJson = JsonSerializer.Serialize(bookingDto.Passengers.Select(p => new
                {
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender
                }));

                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@UserID", bookingDto.UserID),
                    new SqlParameter("@FlightScheduleID", bookingDto.FlightScheduleID),
                    new SqlParameter("@PassengerDetails", passengersJson),
                    new SqlParameter("@CreatedBy", "BookingService")
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<BookingResponseDto>(
                    "sp_CreateFlightBooking", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new BookingResponseDto
                    {
                        Success = false,
                        Message = "Flight booking creation failed. Please try again."
                    };
                }

                _logger.LogInformation("Flight booking created successfully: {BookingReference}", response.BookingReference);
                return response;
            }
            catch (SqlException ex) when (ex.Number >= 50016 && ex.Number <= 50022)
            {
                _logger.LogWarning("Flight booking validation error: {Message}", ex.Message);
                return new BookingResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flight booking for user {UserId}", bookingDto.UserID);
                return new BookingResponseDto
                {
                    Success = false,
                    Message = "Booking creation failed due to a system error. Please try again later."
                };
            }
        }

        public async Task<IEnumerable<UserBookingDto>> GetUserBookingsAsync(int userId, string? bookingType = null, string? status = null)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@BookingType", (object?)bookingType ?? DBNull.Value),
                    new SqlParameter("@Status", (object?)status ?? DBNull.Value)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<UserBookingDto>(
                    "sp_GetUserBookings", parameters);

                _logger.LogInformation("Retrieved {Count} bookings for user {UserId}", result.Count(), userId);
                return result;
            }
            catch (SqlException ex) when (ex.Number == 50009)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                throw new ArgumentException("User not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user {UserId}", userId);
                throw;
            }
        }

        public async Task<BookingDetailsDto> GetBookingDetailsAsync(int bookingId, int userId)
        {
            try
            {
                // Get booking basic details
                var booking = await _unitOfWork.Bookings.FirstOrDefaultAsync(b => 
                    b.BookingID == bookingId && b.UserID == userId && b.IsActive);

                if (booking == null)
                {
                    throw new ArgumentException("Booking not found or access denied.");
                }

                // Get passengers
                var passengers = await _unitOfWork.BookingPassengers.FindAsync(bp => 
                    bp.BookingID == bookingId && bp.IsActive);

                // Get payments
                var payments = await _unitOfWork.Payments.FindAsync(p => 
                    p.BookingID == bookingId && p.IsActive);

                // Get payment modes for payments
                var paymentModes = await _unitOfWork.PaymentModes.GetAllAsync();
                var paymentModeDict = paymentModes.ToDictionary(pm => pm.PaymentModeID, pm => pm.ModeName);

                // Get booking status
                var status = await _unitOfWork.BookingStatusTypes.GetByIdAsync(booking.StatusID);

                var bookingDetails = new BookingDetailsDto
                {
                    BookingID = booking.BookingID,
                    BookingReference = booking.BookingReference,
                    BookingType = booking.BookingType,
                    Status = status?.StatusName ?? "Unknown",
                    TotalAmount = booking.TotalAmount,
                    BookingDate = booking.BookingDate,
                    Passengers = passengers.Select(p => new BookingPassengerDto
                    {
                        PassengerID = p.PassengerID,
                        Name = p.Name,
                        Age = p.Age,
                        Gender = p.Gender,
                        IsSeniorCitizen = p.IsSeniorCitizen,
                        SeatNumber = p.SeatNumber
                    }).ToList(),
                    Payments = payments.Select(p => new PaymentDto
                    {
                        PaymentID = p.PaymentID,
                        Amount = p.Amount,
                        Status = p.Status,
                        TransactionRef = p.TransactionRef,
                        PaymentDate = p.PaymentDate,
                        PaymentMode = paymentModeDict.GetValueOrDefault(p.PaymentModeID, "Unknown")
                    }).ToList()
                };

                // TODO: Add journey details based on booking type (train/flight schedule info)

                _logger.LogInformation("Retrieved booking details for booking {BookingId}", bookingId);
                return bookingDetails;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking details for booking {BookingId}", bookingId);
                throw;
            }
        }

        public async Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int userId, string? reason = null)
        {
            try
            {
                // Verify booking belongs to user
                var booking = await _unitOfWork.Bookings.FirstOrDefaultAsync(b => 
                    b.BookingID == bookingId && b.UserID == userId && b.IsActive);

                if (booking == null)
                {
                    throw new ArgumentException("Booking not found or access denied.");
                }

                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@BookingID", bookingId),
                    new SqlParameter("@CancellationReason", (object?)reason ?? DBNull.Value),
                    new SqlParameter("@CancelledBy", $"User_{userId}")
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<CancellationResponseDto>(
                    "sp_CancelBookingAndPromoteWaitlist", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new CancellationResponseDto
                    {
                        Success = false,
                        Message = "Booking cancellation failed. Please try again."
                    };
                }

                _logger.LogInformation("Booking cancelled successfully: {BookingId}", bookingId);
                return response;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (SqlException ex) when (ex.Number >= 50023 && ex.Number <= 50034)
            {
                _logger.LogWarning("Booking cancellation validation error: {Message}", ex.Message);
                return new CancellationResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
                return new CancellationResponseDto
                {
                    Success = false,
                    Message = "Booking cancellation failed due to a system error. Please try again later."
                };
            }
        }
    }
}