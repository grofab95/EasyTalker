using System.Threading.Tasks;
using EasyTalker.Api.Dto;
using EasyTalker.Api.Requests;
using EasyTalker.Database.Entities;

namespace EasyTalker.Api.Authentication.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResultDto> Authenticate(string username, string password, string ipAddress);
    Task<AuthenticationResultDto> RefreshToken(string token);
    Task RevokeToken(string token);
    
    Task<string> GetAccessToken(UserDb user);
    RefreshTokenDb GetRefreshToken();
}