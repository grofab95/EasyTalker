using System;
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
using EasyTalker.Core.Dto.User;
using EasyTalker.Database;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Api.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string PermissionClaim = "https://easytalker.pl/identity/claims/permission";
        
    private readonly UserManager<UserDb> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITokenHandler _tokenHandler;
    private readonly IMapper _mapper;

    public AuthenticationService(UserManager<UserDb> userManager,
        RoleManager<IdentityRole> roleManager, 
        IServiceScopeFactory serviceScopeFactory,
        ITokenHandler tokenHandler, 
        IMapper mapper,
        EasyTalkerContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _serviceScopeFactory = serviceScopeFactory;
        _tokenHandler = tokenHandler;
        _mapper = mapper;

        try
        {
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            
        }
    }

    public async Task<AuthenticationResultDto> Authenticate(string username, string password, string ipAddress)
    {
        var user = await _userManager.FindByNameAsync(username)
                   ?? throw new Exception("Login not found");

        if (!await _userManager.CheckPasswordAsync(user, password))
            throw new Exception("Incorrect password");
            
        var accessToken = await GetAccessToken(user);
        var refreshToken = GetRefreshToken();

        user.RefreshTokens ??= new List<RefreshTokenDb>();
            
        user.RefreshTokens.Add(refreshToken);
            
        await _userManager.UpdateAsync(user);

        return new AuthenticationResultDto(_mapper.Map<UserDto>(user), accessToken, refreshToken.Token);
    }

    public async Task<AuthenticationResultDto> RefreshToken(string token)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var user = await dbContext.Users
                       //.Include(x => x.RefreshTokens)
                       .SingleOrDefaultAsync(x => x.RefreshTokens.Any(y => y.Token == token))
                   ?? throw new Exception("Invalid token");

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        if (!refreshToken.IsActive)
            throw new Exception("Invalid token");

        var newRefreshToken = GetRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        user.RefreshTokens.Remove(refreshToken);
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();

        var jwtToken = await GetAccessToken(user);

        return new AuthenticationResultDto(_mapper.Map<UserDto>(user), jwtToken, newRefreshToken.Token);
    }

    public async Task RevokeToken(string token)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var user = await dbContext.Users
                      //.Include(x => x.RefreshTokens)
                      .SingleOrDefaultAsync(x => x.RefreshTokens.Any(y => y.Token == token))
                  ?? throw new Exception("Invalid token");
        
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        if (!refreshToken.IsActive)
            throw new Exception("Invalid token");
        
        refreshToken.RevokedAt = DateTime.Now;

        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
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

        var currentDateTime = DateTime.Now;
            
        return await _tokenHandler.GenerateAccessToken(
            claims,
            currentDateTime,
            currentDateTime.AddMinutes(30) // to options
        );
    }

    public RefreshTokenDb GetRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rngCrypto = new RNGCryptoServiceProvider();
            
        rngCrypto.GetBytes(randomBytes);

        return new RefreshTokenDb
        {
            Token = Convert.ToBase64String(randomBytes),
            CreatedAt = DateTime.Now,
            ExpiredAt = DateTime.Now.AddDays(1)
        };
    }
}