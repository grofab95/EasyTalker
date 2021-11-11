using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace EasyTalker.Api.Authentication.Handlers
{
    public class TokenHandler : ITokenHandler
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public TokenHandler()
        {
            
        }

        public Task<string> GenerateAccessToken(IEnumerable<Claim> claims, DateTimeOffset notBefore,
            DateTimeOffset expiresAt)
        {
            return Task.FromResult(_jwtSecurityTokenHandler
                .WriteToken(new JwtSecurityToken(
                    "http://easytalker.pl",
                    "http://easytalker.pl",
                    claims,
                    notBefore.UtcDateTime,
                    expiresAt.UtcDateTime,
                    _signingCredentials
                )));
        }

        public ClaimsPrincipal ValidateToken(string token, bool validateLifetime)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,

                ValidIssuer = "http://easytalker.pl",
                ValidAudience = "http://easytalker.pl",

                IssuerSigningKey = _symmetricSecurityKey,
                RoleClaimType = ClaimTypes.Role
            };

            try
            {
                return _jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        public Task<string> GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rngCrypto = new RNGCryptoServiceProvider();
            
            rngCrypto.GetBytes(randomBytes);

            var randomValue = Convert.ToBase64String(randomBytes);

            return Task.FromResult(randomValue);
        }
    }
}