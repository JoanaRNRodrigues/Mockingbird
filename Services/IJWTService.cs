using MockingBird.Models;

namespace MockingBird.Services
{
    public interface IJWTService
    {
        string GenerateToken(string key, string issuer, string audience, UsersViewModel user);
        bool IsTokenValid(string key, string issuer, string audience, string token);
        string GetJWTTokenClaim(string? token);
    }
}
