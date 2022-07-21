using System.Security.Claims;

namespace EasyTalker.Authentication.Handlers;

public interface ITokenHandler
{
    Task<string> GenerateAccessToken(IEnumerable<Claim> claims, DateTimeOffset notBefore, DateTimeOffset expiresAt);
    ClaimsPrincipal ValidateToken(string token, bool validateLifetime);
}