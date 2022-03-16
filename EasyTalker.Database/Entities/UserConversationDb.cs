using System;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class UserConversationDb : EntityDb
{
    public string UserId { get; set; }
    public long ConversationId { get; set; }
    public ConversationAccessStatus AccessStatus { get; set; }
    public DateTime LastSeenAt { get; set; }

    public UserConversationDb(string userId, long conversationId)
    {
        UserId = userId;
        ConversationId = conversationId;
        AccessStatus = ConversationAccessStatus.ReadAndWrite;
    }
}

public class UserConversationDbConfiguration : IEntityTypeConfiguration<UserConversationDb>
{
    public void Configure(EntityTypeBuilder<UserConversationDb> builder)
    {
        builder
            .Property(b => b.CreatedAt )
            .HasDefaultValueSql(SqlCommands.GetDate);

        builder.HasIndex(x => new {x.ConversationId, x.UserId}).IsUnique();
        
        builder
            .Property(x => x.AccessStatus)
            .HasConversion(
                v => v.ToString(), 
                v => (ConversationAccessStatus) Enum.Parse(typeof(ConversationAccessStatus), v));
    }
}