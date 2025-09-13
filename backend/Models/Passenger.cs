using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public class Passenger : BaseEntity
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SeatNumber { get; set; }

        [MaxLength(20)]
        public string? IdProofType { get; set; }

        [MaxLength(50)]
        public string? IdProofNumber { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; } = null!;
    }
}