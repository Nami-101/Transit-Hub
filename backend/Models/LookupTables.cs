using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitHub.Models
{
    [Table("TrainQuotaTypes")]
    public class TrainQuotaType : BaseEntity
    {
        [Key]
        public int QuotaTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string QuotaName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();
    }

    [Table("BookingStatusTypes")]
    public class BookingStatusType : BaseEntity
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        [MaxLength(50)]
        public string StatusName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    [Table("PaymentModes")]
    public class PaymentMode : BaseEntity
    {
        [Key]
        public int PaymentModeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ModeName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    [Table("TrainClasses")]
    public class TrainClass : BaseEntity
    {
        [Key]
        public int TrainClassID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClassName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();
    }

    [Table("FlightClasses")]
    public class FlightClass : BaseEntity
    {
        [Key]
        public int FlightClassID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClassName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    }
}