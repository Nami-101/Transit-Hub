using System.ComponentModel.DataAnnotations;

namespace TransitHub.Models.DTOs
{
    // Registration DTOs
    public class UserRegistrationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }
    }

    public class UserRegistrationResponseDto
    {
        public int UserID { get; set; }
        public string VerificationToken { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    // Login DTOs
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class UserLoginResponseDto
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsSeniorCitizen { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Token { get; set; } // JWT token will be added by controller
    }

    // Verification DTOs
    public class VerificationResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    // Profile DTOs
    public class UserProfileResponseDto
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsSeniorCitizen { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int WaitlistedBookings { get; set; }
    }

    public class UpdateUserProfileDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }
    }

    public class UpdateProfileResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}