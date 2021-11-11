using System.Threading.Tasks;
using EasyTalker.Database.Entities;

namespace EasyTalker.Api.Authentication.Services
{
    public interface IAuthenticationService
    {
        Task<string> GetAccessToken(UserDb user);
        RefreshToken GetRefreshToken();
    }
}