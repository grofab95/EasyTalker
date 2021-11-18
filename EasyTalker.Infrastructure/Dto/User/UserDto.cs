using System.Collections;

namespace EasyTalker.Infrastructure.Dto.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
        public string IsActive { get; set; }
        public IEnumerable Roles { get; set; }
    }
}