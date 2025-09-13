using System.ComponentModel.DataAnnotations;

namespace TransitHub.Models.DTOs
{
    // Passenger DTOs
    public class PassengerDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty; // Male, Female, Other
    }

    // Train Booking DTOs
    public class CreateTrainBookingDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int TrainScheduleID { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(6)]
        public List<PassengerDto> Passengers { get; set; } = new List<PassengerDto>();
    }

    // Flight Booking DTOs
    public class CreateFlightBookingDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int FlightScheduleID { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(6)]
        public List<PassengerDto> Passengers { get; set; } = new List<PassengerDto>();
    }

    // Booking Response DTOs
    public class BookingResponseDto
    {
        public int BookingID { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    // User Bookings DTOs
    public class UserBookingDto
    {
        public int BookingID { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string BookingType { get; set; } = string.Empty;
        public int TotalPassengers { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Train details (if applicable)
        public string? TrainName { get; set; }
        public string? TrainNumber { get; set; }
        public string? SourceStation { get; set; }
        public string? DestinationStation { get; set; }
        public DateOnly? TravelDate { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string? TrainClass { get; set; }

        // Flight details (if applicable)
        public string? FlightNumber { get; set; }
        public string? Airline { get; set; }
        public string? SourceAirport { get; set; }
        public string? DestinationAirport { get; set; }
        public DateOnly? FlightDate { get; set; }
        public DateTime? FlightDepartureTime { get; set; }
        public DateTime? FlightArrivalTime { get; set; }
        public string? FlightClass { get; set; }

        // Waitlist info
        public int? WaitlistPosition { get; set; }
    }

    // Booking Details DTOs
    public class BookingDetailsDto
    {
        public int BookingID { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string BookingType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
        
        public List<BookingPassengerDto> Passengers { get; set; } = new List<BookingPassengerDto>();
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        
        // Journey details will be populated based on booking type
        public object? JourneyDetails { get; set; }
    }

    public class BookingPassengerDto
    {
        public int PassengerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public bool IsSeniorCitizen { get; set; }
        public string? SeatNumber { get; set; }
    }

    public class PaymentDto
    {
        public int PaymentID { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TransactionRef { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
    }

    // Cancellation DTOs
    public class CancellationResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int SeatsReleased { get; set; }
        public bool Success { get; set; }
    }

    // Payment Processing DTOs
    public class ProcessPaymentDto
    {
        [Required]
        public int BookingID { get; set; }

        [Required]
        public int PaymentModeID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string? TransactionRef { get; set; }
    }

    public class PaymentResponseDto
    {
        public int PaymentID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}