using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public class Flight : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string FlightNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Airline { get; set; } = string.Empty;

        [Required]
        public int SourceStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        public decimal BaseFare { get; set; }

        public DateTime TravelDate { get; set; }

        // Navigation properties
        public virtual Station SourceStation { get; set; } = null!;
        public virtual Station DestinationStation { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}