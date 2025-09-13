using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public class TrainSchedule : BaseEntity
    {
        [Required]
        public int TrainId { get; set; }

        [Required]
        public int TrainClassId { get; set; }

        [Required]
        public int SourceStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public int AvailableSeats { get; set; }

        public decimal Fare { get; set; }

        public DateTime TravelDate { get; set; }

        // Navigation properties
        public virtual Train Train { get; set; } = null!;
        public virtual TrainClass TrainClass { get; set; } = null!;
        public virtual Station SourceStation { get; set; } = null!;
        public virtual Station DestinationStation { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}