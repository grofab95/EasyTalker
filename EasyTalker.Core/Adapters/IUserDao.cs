using System.Threading.Tasks;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Core.Adapters
{
    public interface IUserDao
    {
        Task<UserDto> RegisterUser(string username, string password);
    }
}