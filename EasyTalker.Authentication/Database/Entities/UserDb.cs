using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace EasyTalker.Authentication.Database.Entities;

public class UserDb : IdentityUser
{
    public bool IsActive { get; set; }
    public bool IsOnline { get; set; }
        
    [JsonIgnore]
    public List<RefreshTokenDb> RefreshTokens { get; set; }
}