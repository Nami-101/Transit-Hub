using TransitHub.Models.DTOs;

namespace TransitHub.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse> LogoutAsync(string userId);
        Task<ApiResponse> RegisterAsync(RegisterRequest request);
    }
}