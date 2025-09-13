using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("Payments")]
    public class Payment : BaseEntity
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int BookingID { get; set; }

        [Required]
        public int PaymentModeID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // Pending, Success, Failed, Refunded

        [MaxLength(100)]
        public string? TransactionRef { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("BookingID")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("PaymentModeID")]
        public virtual PaymentMode PaymentMode { get; set; } = null!;
    }

    [Table("WaitlistQueue")]
    public class WaitlistQueue : BaseEntity
    {
        [Key]
        public int WaitlistID { get; set; }

        [Required]
        public int BookingID { get; set; }

        [Required]
        [MaxLength(10)]
        public string ScheduleType { get; set; } = string.Empty; // Train, Flight

        public int? TrainScheduleID { get; set; }

        public int? FlightScheduleID { get; set; }

        [Required]
        public int QueuePosition { get; set; }

        [Required]
        public int Priority { get; set; } = 1; // Higher number = higher priority

        // Navigation Properties
        [ForeignKey("BookingID")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("TrainScheduleID")]
        public virtual TrainSchedule? TrainSchedule { get; set; }

        [ForeignKey("FlightScheduleID")]
        public virtual FlightSchedule? FlightSchedule { get; set; }
    }

    [Table("Cancellations")]
    public class Cancellation
    {
        [Key]
        public int CancellationID { get; set; }

        [Required]
        public int BookingID { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal RefundAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CancellationFee { get; set; } = 0;

        [Required]
        [MaxLength(100)]
        public string CancelledBy { get; set; } = string.Empty;

        [Required]
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "System";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("BookingID")]
        public virtual Booking Booking { get; set; } = null!;
    }
}