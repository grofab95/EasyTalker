using System.Threading.Tasks;
using EasyTalker.Core.Dto.User;

namespace EasyTalker.Core.Adapters;

public interface IUserStore
{
    Task<UserDto> RegisterUser(string username, string email, string password);
    Task<UserDto> GetById(string userId);
    Task<UserDto[]> GetAll();

    Task UpdateUserConnectionStatus(string userId, bool isOnline);
    Task SetAllUsersAsOffline();
}