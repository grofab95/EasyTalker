using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class ConversationDb : EntityDb
{
    public string CreatorId { get; set; }
    public string Title { get; set; }
}

public class ConversationDbConfiguration : IEntityTypeConfiguration<ConversationDb>
{
    public void Configure(EntityTypeBuilder<ConversationDb> builder)
    {
        builder
            .Property(b => b.CreatedAt )
            .HasDefaultValueSql("getdate()");

        builder
            .HasIndex(x => new {x.CreatorId, x.Title})
            .IsUnique();
    }
}