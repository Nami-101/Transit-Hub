using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using TransitHub.Models.DTOs;
using TransitHub.Repositories.Interfaces;
using TransitHub.Services.Interfaces;

namespace TransitHub.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserRegistrationResponseDto> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                // Hash the password
                var passwordHash = HashPassword(registrationDto.Password);

                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@Name", registrationDto.Name),
                    new SqlParameter("@Email", registrationDto.Email),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@Phone", registrationDto.Phone),
                    new SqlParameter("@Age", registrationDto.Age),
                    new SqlParameter("@CreatedBy", "UserRegistration")
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<UserRegistrationResponseDto>(
                    "sp_RegisterUser", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new UserRegistrationResponseDto
                    {
                        Success = false,
                        Message = "Registration failed. Please try again."
                    };
                }

                _logger.LogInformation("User registered successfully: {Email}", registrationDto.Email);
                return response;
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                _logger.LogWarning("Registration failed - Email already exists: {Email}", registrationDto.Email);
                return new UserRegistrationResponseDto
                {
                    Success = false,
                    Message = "Email address is already registered."
                };
            }
            catch (SqlException ex) when (ex.Number == 50002)
            {
                _logger.LogWarning("Registration failed - Phone already exists: {Phone}", registrationDto.Phone);
                return new UserRegistrationResponseDto
                {
                    Success = false,
                    Message = "Phone number is already registered."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Email}", registrationDto.Email);
                return new UserRegistrationResponseDto
                {
                    Success = false,
                    Message = "Registration failed due to a system error. Please try again later."
                };
            }
        }

        public async Task<UserLoginResponseDto> LoginUserAsync(UserLoginDto loginDto)
        {
            try
            {
                // Hash the password
                var passwordHash = HashPassword(loginDto.Password);

                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@Email", loginDto.Email),
                    new SqlParameter("@PasswordHash", passwordHash)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<UserLoginResponseDto>(
                    "sp_LoginUser", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new UserLoginResponseDto
                    {
                        Success = false,
                        Message = "Login failed. Please check your credentials."
                    };
                }

                _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
                return response;
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                _logger.LogWarning("Login failed - Invalid credentials: {Email}", loginDto.Email);
                return new UserLoginResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            catch (SqlException ex) when (ex.Number == 50005)
            {
                _logger.LogWarning("Login failed - Email not verified: {Email}", loginDto.Email);
                return new UserLoginResponseDto
                {
                    Success = false,
                    Message = "Please verify your email address before logging in."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login: {Email}", loginDto.Email);
                return new UserLoginResponseDto
                {
                    Success = false,
                    Message = "Login failed due to a system error. Please try again later."
                };
            }
        }

        public async Task<VerificationResponseDto> VerifyEmailAsync(string token)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@Token", token)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<VerificationResponseDto>(
                    "sp_VerifyEmail", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new VerificationResponseDto
                    {
                        Success = false,
                        Message = "Email verification failed."
                    };
                }

                _logger.LogInformation("Email verified successfully for token: {Token}", token);
                return response;
            }
            catch (SqlException ex) when (ex.Number >= 50006 && ex.Number <= 50008)
            {
                _logger.LogWarning("Email verification failed: {Message}", ex.Message);
                return new VerificationResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification: {Token}", token);
                return new VerificationResponseDto
                {
                    Success = false,
                    Message = "Email verification failed due to a system error."
                };
            }
        }

        public async Task<UserProfileResponseDto> GetUserProfileAsync(int userId)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@UserID", userId)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<UserProfileResponseDto>(
                    "sp_GetUserProfile", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    throw new ArgumentException("User not found.");
                }

                return response;
            }
            catch (SqlException ex) when (ex.Number == 50009)
            {
                _logger.LogWarning("User profile not found: {UserId}", userId);
                throw new ArgumentException("User not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile: {UserId}", userId);
                throw;
            }
        }

        public async Task<UpdateProfileResponseDto> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@Name", updateDto.Name),
                    new SqlParameter("@Phone", updateDto.Phone),
                    new SqlParameter("@Age", updateDto.Age),
                    new SqlParameter("@UpdatedBy", "UserUpdate")
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<UpdateProfileResponseDto>(
                    "sp_UpdateUserProfile", parameters);

                var response = result.FirstOrDefault();
                if (response == null)
                {
                    return new UpdateProfileResponseDto
                    {
                        Success = false,
                        Message = "Profile update failed."
                    };
                }

                _logger.LogInformation("User profile updated successfully: {UserId}", userId);
                return response;
            }
            catch (SqlException ex) when (ex.Number == 50002)
            {
                _logger.LogWarning("Profile update failed - Phone already exists: {Phone}", updateDto.Phone);
                return new UpdateProfileResponseDto
                {
                    Success = false,
                    Message = "Phone number is already registered to another user."
                };
            }
            catch (SqlException ex) when (ex.Number == 50009)
            {
                _logger.LogWarning("Profile update failed - User not found: {UserId}", userId);
                return new UpdateProfileResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
                return new UpdateProfileResponseDto
                {
                    Success = false,
                    Message = "Profile update failed due to a system error."
                };
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}