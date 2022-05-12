using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using EasyTalker.Authentication.Database;
using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Authentication.Handlers;
using EasyTalker.Core.Dto;
using EasyTalker.Core.Dto.Authentications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string PermissionClaim = "https://easytalker.pl/identity/claims/permission";
        
    private readonly UserManager<UserDb> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITokenHandler _tokenHandler;

    public AuthenticationService(UserManager<UserDb> userManager,
        RoleManager<IdentityRole> roleManager, 
        IServiceScopeFactory serviceScopeFactory,
        ITokenHandler tokenHandler)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _serviceScopeFactory = serviceScopeFactory;
        _tokenHandler = tokenHandler;
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
        return new AuthenticationResultDto(user.ToUserDto(), accessToken, refreshToken.Token);
    }

    public async Task<AuthenticationResultDto> RefreshToken(string token)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerAuthenticationContext>();

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
        return new AuthenticationResultDto(user.ToUserDto(), jwtToken, newRefreshToken.Token);
    }

    public async Task RevokeToken(string token)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerAuthenticationContext>();
        
        var user = await dbContext.Users
                      .Include(x => x.RefreshTokens)
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
            currentDateTime.AddMinutes(30)
        );
    }

    public RefreshTokenDb GetRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rngCrypto = RandomNumberGenerator.Create();
        rngCrypto.GetBytes(randomBytes);
        return new RefreshTokenDb
        {
            Token = Convert.ToBase64String(randomBytes),
            CreatedAt = DateTime.Now,
            ExpiredAt = DateTime.Now.AddDays(1)
        };
    }
}