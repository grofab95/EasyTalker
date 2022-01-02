using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class FileDb : EntityDb
{
    public string FileName { get; set; }
    public string Path { get; set; }
}

public class FileDbConfiguration : IEntityTypeConfiguration<FileDb>
{
    public void Configure(EntityTypeBuilder<FileDb> builder)
    {
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.Path).IsRequired();
    }
}