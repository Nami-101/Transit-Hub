using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public class TrainClass : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public decimal BaseFareMultiplier { get; set; } = 1.0m;

        // Navigation properties
        public virtual ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();
    }
}