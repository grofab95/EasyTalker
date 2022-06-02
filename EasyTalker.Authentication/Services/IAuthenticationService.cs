using EasyTalker.Core.Dto.Authentications;

namespace EasyTalker.Authentication.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResultDto> Authenticate(string username, string password);
}