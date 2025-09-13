using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("Bookings")]
    public class Booking : BaseEntity
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        [MaxLength(20)]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(10)]
        public string BookingType { get; set; } = string.Empty; // Train, Flight

        public int? TrainScheduleID { get; set; }

        public int? FlightScheduleID { get; set; }

        [Required]
        public int StatusID { get; set; }

        [Required]
        [Range(1, 6)]
        public int TotalPassengers { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("TrainScheduleID")]
        public virtual TrainSchedule? TrainSchedule { get; set; }

        [ForeignKey("FlightScheduleID")]
        public virtual FlightSchedule? FlightSchedule { get; set; }

        [ForeignKey("StatusID")]
        public virtual BookingStatusType Status { get; set; } = null!;

        public virtual ICollection<BookingPassenger> Passengers { get; set; } = new List<BookingPassenger>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<BookingAudit> AuditLogs { get; set; } = new List<BookingAudit>();
        public virtual ICollection<WaitlistQueue> WaitlistQueues { get; set; } = new List<WaitlistQueue>();
        public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    [Table("BookingPassengers")]
    public class BookingPassenger : BaseEntity
    {
        [Key]
        public int PassengerID { get; set; }

        [Required]
        public int BookingID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty; // Male, Female, Other

        [Required]
        public bool IsSeniorCitizen { get; set; } = false;

        [MaxLength(10)]
        public string? SeatNumber { get; set; }

        // Navigation Properties
        [ForeignKey("BookingID")]
        public virtual Booking Booking { get; set; } = null!;
    }
}