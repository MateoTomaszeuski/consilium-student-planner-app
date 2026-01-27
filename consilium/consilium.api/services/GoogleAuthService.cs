using Google.Apis.Auth;
using System.Data;
using Dapper;

namespace Consilium.API.Services;

public class GoogleAuthService
{
    private readonly IDbConnection _db;
    private readonly IConfiguration _config;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(IDbConnection db, IConfiguration config, ILogger<GoogleAuthService> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
    }

    public async Task<GoogleUserInfo?> ValidateGoogleToken(string idToken)
    {
        try
        {
            var clientId = _config["GOOGLE_CLIENT_ID"] ?? throw new Exception("GOOGLE_CLIENT_ID not configured");
            
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name,
                PictureUrl = payload.Picture,
                EmailVerified = payload.EmailVerified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate Google token");
            return null;
        }
    }

    public async Task<User?> GetOrCreateUser(GoogleUserInfo googleInfo)
    {
        // Check if user exists by google_id
        var existingUser = await _db.QueryFirstOrDefaultAsync<User>(
            @"SELECT * FROM ""user"" WHERE google_id = @GoogleId",
            new { googleInfo.GoogleId }
        );

        if (existingUser != null)
        {
            // Update last login
            await _db.ExecuteAsync(
                @"UPDATE ""user"" SET last_login = CURRENT_TIMESTAMP WHERE google_id = @GoogleId",
                new { googleInfo.GoogleId }
            );
            return existingUser;
        }

        // Check if user exists by email (migration path)
        existingUser = await _db.QueryFirstOrDefaultAsync<User>(
            @"SELECT * FROM ""user"" WHERE email = @Email",
            new { googleInfo.Email }
        );

        if (existingUser != null)
        {
            // Update existing user with Google ID
            await _db.ExecuteAsync(
                @"UPDATE ""user"" 
                  SET google_id = @GoogleId, 
                      profile_picture = @PictureUrl,
                      last_login = CURRENT_TIMESTAMP 
                  WHERE email = @Email",
                new 
                { 
                    googleInfo.GoogleId, 
                    googleInfo.PictureUrl, 
                    googleInfo.Email 
                }
            );
            existingUser.GoogleId = googleInfo.GoogleId;
            existingUser.ProfilePicture = googleInfo.PictureUrl;
            return existingUser;
        }

        // Create new user
        var newUser = await _db.QueryFirstAsync<User>(
            @"INSERT INTO ""user"" (email, google_id, displayName, profile_picture, role, created_at, last_login)
              VALUES (@Email, @GoogleId, @Name, @PictureUrl, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
              RETURNING *",
            new 
            { 
                googleInfo.Email, 
                googleInfo.GoogleId, 
                googleInfo.Name, 
                googleInfo.PictureUrl 
            }
        );

        return newUser;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.QueryFirstOrDefaultAsync<User>(
            @"SELECT * FROM ""user"" WHERE email = @Email",
            new { Email = email }
        );
    }

    public async Task DeleteUser(string email)
    {
        await _db.ExecuteAsync(
            @"DELETE FROM ""user"" WHERE email = @Email",
            new { Email = email }
        );
    }
}

public class GoogleUserInfo
{
    public string GoogleId { get; set; } = "";
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string PictureUrl { get; set; } = "";
    public bool EmailVerified { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string GoogleId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? ProfilePicture { get; set; }
    public int Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}
