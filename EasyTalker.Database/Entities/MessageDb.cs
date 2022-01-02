using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class MessageDb : EntityDb
{
    public UserDb Sender { get; set; }
    public UserDb ReceivingUser { get; set; }
    public GroupDb ReceivingGroup { get; set; }
    public FileDb File { get; set; }
    public string Text { get; set; }
    public MessageStatus Status { get; set; }
}

public enum MessageStatus
{
    Send,
    Delivered,
    Read
}

public class MessageDbConfiguration : IEntityTypeConfiguration<MessageDb>
{
    public void Configure(EntityTypeBuilder<MessageDb> builder)
    {
        builder.HasOne(x => x.Sender);
        builder.HasOne(x => x.ReceivingUser);
        builder.HasOne(x => x.ReceivingGroup);
        builder.HasOne(x => x.File);

        builder
            .Property(x => x.Status)
            .HasConversion(
                v => v.ToString(), 
                v => (MessageStatus) Enum.Parse(typeof(MessageStatus), v));
    }
}