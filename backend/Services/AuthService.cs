using Microsoft.AspNetCore.Identity;
using TransitHub.Models.DTOs;
using TransitHub.Services.Interfaces;

namespace TransitHub.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Invalid email or password");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return ApiResponse<LoginResponse>.ErrorResult("Account is locked out");
                    }
                    return ApiResponse<LoginResponse>.ErrorResult("Invalid email or password");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _jwtService.GenerateTokenAsync(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var loginResponse = new LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Match with JWT expiration
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.UserName!, // Adjust based on your user model
                        LastName = "", // Adjust based on your user model
                        Roles = roles.ToList()
                    }
                };

                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                return ApiResponse<LoginResponse>.SuccessResult(loginResponse, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", request.Email);
                return ApiResponse<LoginResponse>.ErrorResult("An error occurred during login");
            }
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // In a real application, you would store refresh tokens in database
                // and validate them here. For now, we'll generate a new token
                // This is a simplified implementation

                return ApiResponse<LoginResponse>.ErrorResult("Refresh token functionality not implemented yet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<LoginResponse>.ErrorResult("An error occurred during token refresh");
            }
        }

        public async Task<ApiResponse> LogoutAsync(string userId)
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User {UserId} logged out successfully", userId);
                return ApiResponse.SuccessResult("Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return ApiResponse.ErrorResult("An error occurred during logout");
            }
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return ApiResponse.ErrorResult("User with this email already exists");
                }

                var user = new IdentityUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true // Set to false if you want email confirmation
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse.ErrorResult("Registration failed", errors);
                }

                // Add user to default role
                await _userManager.AddToRoleAsync(user, "User");

                _logger.LogInformation("User {Email} registered successfully", request.Email);
                return ApiResponse.SuccessResult("Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
                return ApiResponse.ErrorResult("An error occurred during registration");
            }
        }
    }
}