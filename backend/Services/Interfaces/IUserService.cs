using TransitHub.Models.DTOs;

namespace TransitHub.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserRegistrationResponseDto> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<UserLoginResponseDto> LoginUserAsync(UserLoginDto loginDto);
        Task<VerificationResponseDto> VerifyEmailAsync(string token);
        Task<UserProfileResponseDto> GetUserProfileAsync(int userId);
        Task<UpdateProfileResponseDto> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);
    }
}