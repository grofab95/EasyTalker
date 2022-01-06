using System.Collections;

namespace EasyTalker.Core.Dto.User;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsOnline { get; set; }
    public IEnumerable Roles { get; set; }
}