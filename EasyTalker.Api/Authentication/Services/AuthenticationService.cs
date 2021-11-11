using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using EasyTalker.Api.Authentication.Handlers;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace EasyTalker.Api.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string PermissionClaim = "http://fgcode.pl/identity/claims/permission";
        
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenHandler _tokenHandler;

        public AuthenticationService()
        {
            
        }

        public async Task<string> GetAccessToken(UserDb user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPremissions = new List<Claim>();
            foreach (var userRole in userRoles)
            {
                var role = _roleManager.Roles.FirstOrDefault(x => x.Name == userRole);
                if (role == null)
                    continue;
                
                userPremissions.AddRange(await _roleManager.GetClaimsAsync(role));
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.UserData, JsonSerializer.Serialize(new { isActive = user.IsActive })),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            claims.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));
            claims.AddRange(userPremissions.Select(x => new Claim(PermissionClaim, x.Value)));

            var currentDateTime = DateTime.UtcNow;
            
            return await _tokenHandler.GenerateAccessToken(
                claims,
                currentDateTime,
                currentDateTime.AddMinutes(30) // to options
            );
        }

        public RefreshToken GetRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rngCrypto = new RNGCryptoServiceProvider();
            
            rngCrypto.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiredAt = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}