using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class GroupDb : EntityDb
{
    public string Name { get; set; }
    public long AdminUserId { get; set; }
    public IEnumerable<UserDb> Users { get; set; }
}

public class GroupDbConfiguration : IEntityTypeConfiguration<GroupDb>
{
    public void Configure(EntityTypeBuilder<GroupDb> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.AdminUserId).IsRequired();
        
        // builder
        //     .HasMany(x => x.Users)
        //     .WithMany(x => x.Groups);

    }
}