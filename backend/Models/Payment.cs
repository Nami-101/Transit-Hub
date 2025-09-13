using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        NetBanking,
        UPI,
        Wallet
    }

    public class Payment : BaseEntity
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime? PaymentDate { get; set; }

        [MaxLength(200)]
        public string? PaymentGatewayResponse { get; set; }

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; } = null!;
    }
}