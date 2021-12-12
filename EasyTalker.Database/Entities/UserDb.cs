using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace EasyTalker.Database.Entities;

public class UserDb : IdentityUser
{
    public bool IsActive { get; set; }
        
    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; }
}