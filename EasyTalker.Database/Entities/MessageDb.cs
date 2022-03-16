using System;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class MessageDb : EntityDb
{
    public string SenderId { get; set; }
    public long ConversationId { get; set; }
    
    public string Text { get; set; }
    public MessageStatus Status { get; set; }
}

public class MessageDbConfiguration : IEntityTypeConfiguration<MessageDb>
{
    public void Configure(EntityTypeBuilder<MessageDb> builder)
    {
        builder
            .Property(b => b.CreatedAt )
            .HasDefaultValueSql(SqlCommands.GetDate);
        
        builder
            .Property(x => x.Status)
            .HasConversion(
                v => v.ToString(), 
                v => (MessageStatus) Enum.Parse(typeof(MessageStatus), v));
    }
}