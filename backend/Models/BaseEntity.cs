using System.ComponentModel.DataAnnotations;

namespace TransitHub.Models
{
    public abstract class BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "System";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}