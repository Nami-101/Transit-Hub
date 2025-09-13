using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TrainBooking.Services
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(IdentityUser user, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
}