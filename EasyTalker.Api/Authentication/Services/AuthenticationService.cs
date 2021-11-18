﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Api.Authentication.Handlers;
using EasyTalker.Api.Dto;
using EasyTalker.Api.Requests;
using EasyTalker.Database.Entities;
using EasyTalker.Infrastructure.Dto;
using EasyTalker.Infrastructure.Dto.User;
using Microsoft.AspNetCore.Identity;

namespace EasyTalker.Api.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string PermissionClaim = "http://easytalker.pl/identity/claims/permission";
        
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly IMapper _mapper;

        public AuthenticationService(UserManager<UserDb> userManager,
            RoleManager<IdentityRole> roleManager, 
            ITokenHandler tokenHandler, 
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenHandler = tokenHandler;
            _mapper = mapper;
        }

        public async Task<AuthenticationResultDto> Authenticate(string username, string password, string ipAddress)
        {
            var user = await _userManager.FindByNameAsync(username)
                ?? throw new Exception("Login not found");

            var accessToken = await GetAccessToken(user);
            var refreshToken = GetRefreshToken();

            user.RefreshTokens ??= new List<RefreshToken>();
            
            user.RefreshTokens.Add(refreshToken);
            
            await _userManager.UpdateAsync(user);

            return new AuthenticationResultDto(_mapper.Map<UserDto>(user), accessToken, refreshToken.Token);
        }
        
        public async Task<string> GetAccessToken(UserDb user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = new List<Claim>();
            foreach (var userRole in userRoles)
            {
                var role = _roleManager.Roles.FirstOrDefault(x => x.Name == userRole);
                if (role == null)
                    continue;
                
                userPermissions.AddRange(await _roleManager.GetClaimsAsync(role));
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.UserData, JsonSerializer.Serialize(new { isActive = user.IsActive })),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            claims.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));
            claims.AddRange(userPermissions.Select(x => new Claim(PermissionClaim, x.Value)));

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