using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class UserDb : IdentityUser
{
    public bool IsActive { get; set; }
    public IEnumerable<GroupDb> Groups { get; set; }
        
    [JsonIgnore]
    public List<RefreshTokenDb> RefreshTokens { get; set; }
}

public class UserDbConfiguration : IEntityTypeConfiguration<UserDb>
{
    public void Configure(EntityTypeBuilder<UserDb> builder)
    {
        
    }
}