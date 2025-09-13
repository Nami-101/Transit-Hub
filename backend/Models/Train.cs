using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public class Train : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string TrainNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int SourceStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }

        public int TotalSeats { get; set; }

        public decimal BaseFare { get; set; }

        // Navigation properties
        public virtual Station SourceStation { get; set; } = null!;
        public virtual Station DestinationStation { get; set; } = null!;
        public virtual ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}