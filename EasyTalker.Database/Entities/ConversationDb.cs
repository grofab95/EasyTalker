using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class ConversationDb : EntityDb
{
    public string Title { get; set; }
}

public class ConversationDbConfiguration : IEntityTypeConfiguration<ConversationDb>
{
    public void Configure(EntityTypeBuilder<ConversationDb> builder)
    {
        
    }
}