using System.Threading.Tasks;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Core.Adapters;

public interface IUserStore
{
    Task<UserDto> RegisterUser(string username, string email, string password);
    Task<UserDto> GetById(string userId);
}