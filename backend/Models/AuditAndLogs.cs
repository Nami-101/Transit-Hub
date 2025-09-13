using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("GmailVerificationTokens")]
    public class GmailVerificationToken
    {
        [Key]
        public int TokenID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public bool IsUsed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
    }

    [Table("BookingAudit")]
    public class BookingAudit
    {
        [Key]
        public int AuditID { get; set; }

        [Required]
        public int BookingID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ActionBy { get; set; } = string.Empty;

        [Required]
        public DateTime ActionTime { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Details { get; set; }

        public string? OldValues { get; set; } // JSON format

        public string? NewValues { get; set; } // JSON format

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "System";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("BookingID")]
        public virtual Booking Booking { get; set; } = null!;
    }

    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Unread"; // Unread, Read

        public int? RelatedBookingID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("RelatedBookingID")]
        public virtual Booking? RelatedBooking { get; set; }
    }

    [Table("SystemLogs")]
    public class SystemLog
    {
        [Key]
        public int LogID { get; set; }

        [Required]
        [MaxLength(20)]
        public string LogLevel { get; set; } = string.Empty; // Error, Warning, Info, Debug

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        public int? UserID { get; set; }

        [MaxLength(500)]
        public string? RequestPath { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? IPAddress { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}