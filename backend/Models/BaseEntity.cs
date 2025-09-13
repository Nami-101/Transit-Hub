using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string? UpdatedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}