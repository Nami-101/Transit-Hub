using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("Airports")]
    public class Airport : BaseEntity
    {
        [Key]
        public int AirportID { get; set; }

        [Required]
        [MaxLength(150)]
        public string AirportName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(5)]
        public string Code { get; set; } = string.Empty; // IATA Code

        // Navigation Properties
        public virtual ICollection<Flight> SourceFlights { get; set; } = new List<Flight>();
        public virtual ICollection<Flight> DestinationFlights { get; set; } = new List<Flight>();
    }
}