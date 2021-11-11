using Microsoft.AspNetCore.Identity;

namespace EasyTalker.Database.Entities
{
    public class UserDb : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}