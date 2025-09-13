using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public class Station : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        public string? Country { get; set; } = "India";

        // Navigation properties
        public virtual ICollection<TrainSchedule> DepartureTrains { get; set; } = new List<TrainSchedule>();
        public virtual ICollection<TrainSchedule> ArrivalTrains { get; set; } = new List<TrainSchedule>();
        public virtual ICollection<Flight> DepartureFlights { get; set; } = new List<Flight>();
        public virtual ICollection<Flight> ArrivalFlights { get; set; } = new List<Flight>();
    }
}