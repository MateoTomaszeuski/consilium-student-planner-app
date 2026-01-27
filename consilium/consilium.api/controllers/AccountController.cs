using Consilium.API.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request) {
        _logger.LogInformation("Google sign-in attempt");

        if (string.IsNullOrEmpty(request.IdToken)) {
            return BadRequest("ID token is required");
        }

        var googleUser = await _authService.ValidateGoogleToken(request.IdToken);

        if (googleUser == null) {
            _logger.LogWarning("Invalid Google token");
            return Unauthorized("Invalid Google token");
        }

        if (!googleUser.EmailVerified) {
            _logger.LogWarning("Email not verified for {email}", googleUser.Email);
            return Unauthorized("Email not verified");
        }

        var user = await _authService.GetOrCreateUser(googleUser);

        if (user == null) {
            _logger.LogError("Failed to create/get user for {email}", googleUser.Email);
            return StatusCode(500, "Failed to create user");
        }

        _logger.LogInformation("User {email} signed in successfully", user.Email);

        return Ok(new {
            user.Email,
            user.DisplayName,
            user.ProfilePicture,
            user.Role,
            user.ThemePreference,
            user.Notes,
            Message = "Sign-in successful"
        });
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetCurrentUser([FromQuery] string email) {
        if (string.IsNullOrEmpty(email)) {
            return BadRequest("Email is required");
        }

        var user = await _authService.GetUserByEmail(email);

        if (user == null) {
            return NotFound("User not found");
        }

        return Ok(new {
            user.Email,
            user.DisplayName,
            user.ProfilePicture,
            user.Role,
            user.ThemePreference,
            user.Notes
        });
    }

    [HttpPost("theme")]
    public async Task<IActionResult> UpdateTheme([FromBody] UpdateThemeRequest request) {
        var email = Request.Headers["Email-Auth_Email"].FirstOrDefault();

        if (string.IsNullOrEmpty(email)) {
            return BadRequest("Email header is required");
        }

        if (string.IsNullOrEmpty(request.Theme)) {
            return BadRequest("Theme is required");
        }

        var success = await _authService.UpdateThemePreference(email, request.Theme);

        if (!success) {
            return NotFound("User not found");
        }

        _logger.LogInformation("Theme updated to {theme} for {email}", request.Theme, email);
        return Ok(new { theme = request.Theme });
    }

    [HttpPost("notes")]
    public async Task<IActionResult> UpdateNotes([FromBody] UpdateNotesRequest request) {
        var email = Request.Headers["Email-Auth_Email"].FirstOrDefault();

        if (string.IsNullOrEmpty(email)) {
            return BadRequest("Email header is required");
        }

        var success = await _authService.UpdateNotes(email, request.Notes ?? "");

        if (!success) {
            return NotFound("User not found");
        }

        _logger.LogInformation("Notes updated for {email}", email);
        return Ok(new { notes = request.Notes });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAccount([FromQuery] string email) {
        if (string.IsNullOrEmpty(email)) {
            return BadRequest("Email is required");
        }

        _logger.LogInformation("Deleting account for {email}", email);
        await _authService.DeleteUser(email);
        return Ok("Account deleted successfully");
    }
}

public class GoogleSignInRequest {
    public string IdToken { get; set; } = "";
}

public class UpdateThemeRequest {
    public string Theme { get; set; } = "";
}

public class UpdateNotesRequest {
    public string? Notes { get; set; }
}