using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("Flights")]
    public class Flight : BaseEntity
    {
        [Key]
        public int FlightID { get; set; }

        [Required]
        [MaxLength(10)]
        public string FlightNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Airline { get; set; } = string.Empty;

        [Required]
        public int SourceAirportID { get; set; }

        [Required]
        public int DestinationAirportID { get; set; }

        // Navigation Properties
        [ForeignKey("SourceAirportID")]
        public virtual Airport SourceAirport { get; set; } = null!;

        [ForeignKey("DestinationAirportID")]
        public virtual Airport DestinationAirport { get; set; } = null!;

        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    }
}