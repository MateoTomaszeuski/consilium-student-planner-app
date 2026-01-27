using Microsoft.AspNetCore.Mvc;
using Consilium.API.Services;

namespace Consilium.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase {

    private readonly GoogleAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(GoogleAuthService authService, ILogger<AccountController> logger) {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("google-signin")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request)
    {
        _logger.LogInformation("Google sign-in attempt");

        if (string.IsNullOrEmpty(request.IdToken))
        {
            return BadRequest("ID token is required");
        }

        var googleUser = await _authService.ValidateGoogleToken(request.IdToken);
        
        if (googleUser == null)
        {
            _logger.LogWarning("Invalid Google token");
            return Unauthorized("Invalid Google token");
        }

        if (!googleUser.EmailVerified)
        {
            _logger.LogWarning("Email not verified for {email}", googleUser.Email);
            return Unauthorized("Email not verified");
        }

        var user = await _authService.GetOrCreateUser(googleUser);
        
        if (user == null)
        {
            _logger.LogError("Failed to create/get user for {email}", googleUser.Email);
            return StatusCode(500, "Failed to create user");
        }

        _logger.LogInformation("User {email} signed in successfully", user.Email);

        return Ok(new
        {
            user.Email,
            user.DisplayName,
            user.ProfilePicture,
            user.Role,
            Message = "Sign-in successful"
        });
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetCurrentUser([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email is required");
        }

        var user = await _authService.GetUserByEmail(email);
        
        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(new
        {
            user.Email,
            user.DisplayName,
            user.ProfilePicture,
            user.Role
        });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAccount([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email is required");
        }

        _logger.LogInformation("Deleting account for {email}", email);
        await _authService.DeleteUser(email);
        return Ok("Account deleted successfully");
    }
}

public class GoogleSignInRequest
{
    public string IdToken { get; set; } = "";
}
