using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyTalker.Api.Authentication.Handlers
{
    public interface ITokenHandler
    {
        Task<string> GenerateAccessToken(IEnumerable<Claim> claims, DateTimeOffset notBefore, DateTimeOffset expiresAt);
        Task<string> GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token, bool validateLifetime);
    }
}