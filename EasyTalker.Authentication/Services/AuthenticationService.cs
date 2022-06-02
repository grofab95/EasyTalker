using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Authentication.Handlers;
using EasyTalker.Core.Dto.Authentications;
using Microsoft.AspNetCore.Identity;
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

    public async Task<AuthenticationResultDto> Authenticate(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username)
                   ?? throw new Exception("Login not found");

        if (!await _userManager.CheckPasswordAsync(user, password))
            throw new Exception("Incorrect password");
            
        var accessToken = await GetAccessToken(user);
        await _userManager.UpdateAsync(user);
        return new AuthenticationResultDto(user.ToUserDto(), accessToken);
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
}