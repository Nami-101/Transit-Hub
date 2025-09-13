using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("Stations")]
    public class Station : BaseEntity
    {
        [Key]
        public int StationID { get; set; }

        [Required]
        [MaxLength(100)]
        public string StationName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string StationCode { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<Train> SourceTrains { get; set; } = new List<Train>();
        public virtual ICollection<Train> DestinationTrains { get; set; } = new List<Train>();
    }
}