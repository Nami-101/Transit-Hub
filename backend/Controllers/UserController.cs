using Microsoft.AspNetCore.Mvc;
using TransitHub.Models.DTOs;
using TransitHub.Services.Interfaces;

namespace TransitHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserRegistrationResponseDto>> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.RegisterUserAsync(registrationDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in user registration");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.LoginUserAsync(loginDto);
                
                if (result.Success)
                {
                    // TODO: Generate JWT token here and add to result.Token
                    return Ok(result);
                }
                
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in user login");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Verify user email
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<ActionResult<VerificationResponseDto>> VerifyEmail([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { Message = "Token is required" });
                }

                var result = await _userService.VerifyEmailAsync(token);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in email verification");
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile(int userId)
        {
            try
            {
                // TODO: Add JWT authentication and ensure user can only access their own profile
                var result = await _userService.GetUserProfileAsync(userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile/{userId}")]
        public async Task<ActionResult<UpdateProfileResponseDto>> UpdateProfile(int userId, [FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // TODO: Add JWT authentication and ensure user can only update their own profile
                var result = await _userService.UpdateUserProfileAsync(userId, updateDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error occurred" });
            }
        }
    }
}