using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class UserConversationDb : EntityDb
{
    public string UserId { get; set; }
    public long ConversationId { get; set; }
    public bool HasAccess { get; set; }

    public UserConversationDb(string userId, long conversationId)
    {
        UserId = userId;
        ConversationId = conversationId;
        HasAccess = true;
    }
}

public class UserConversationDbConfiguration : IEntityTypeConfiguration<UserConversationDb>
{
    public void Configure(EntityTypeBuilder<UserConversationDb> builder)
    {
        builder
            .Property(b => b.CreatedAt )
            .HasDefaultValueSql("getdate()");

        builder.HasIndex(x => new {x.ConversationId, x.UserId}).IsUnique();
    }
}