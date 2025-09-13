using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TrainBooking.Models
{
    public enum BookingStatus
    {
        Confirmed,
        Waitlisted,
        Cancelled,
        Completed
    }

    public enum BookingType
    {
        Train,
        Flight
    }

    public class Booking : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public BookingType BookingType { get; set; }

        public int? TrainId { get; set; }
        public int? TrainScheduleId { get; set; }
        public int? FlightId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public DateTime TravelDate { get; set; }

        public int PassengerCount { get; set; }

        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public string? SpecialRequests { get; set; }

        // Navigation properties
        public virtual IdentityUser User { get; set; } = null!;
        public virtual Train? Train { get; set; }
        public virtual TrainSchedule? TrainSchedule { get; set; }
        public virtual Flight? Flight { get; set; }
        public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
        public virtual Payment? Payment { get; set; }
    }
}