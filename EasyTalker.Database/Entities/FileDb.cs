using System;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalker.Database.Entities;

public class FileDb : EntityDb
{
    public string ExternalId { get; set; }
    public string FileName { get; set; }
    public string OwnerId { get; set; }
    public FileStatus FileStatus { get; set; }
    public FileType FileType { get; set; }

    protected FileDb()
    { }
    
    public FileDb(string ownerId, UploadFileDto uploadFileDto, FileType fileType)
    {
        ExternalId = uploadFileDto.ExternalId;
        FileName = uploadFileDto.File.FileName;
        OwnerId = ownerId;
        FileType = fileType;
    }
}

public class FileDbConfiguration : IEntityTypeConfiguration<FileDb>
{
    public void Configure(EntityTypeBuilder<FileDb> builder)
    {
        builder
            .Property(b => b.CreatedAt )
            .HasDefaultValueSql(SqlCommands.GetDate);

        builder
            .HasIndex(x => new {x.ExternalId, x.FileName})
            .IsUnique();

        builder.Property(x => x.OwnerId).IsRequired();
        
        builder
            .Property(x => x.FileStatus)
            .HasConversion(
                v => v.ToString(), 
                v => (FileStatus) Enum.Parse(typeof(FileStatus), v));
        
        builder
            .Property(x => x.FileType)
            .HasConversion(
                v => v.ToString(), 
                v => (FileType) Enum.Parse(typeof(FileType), v));
    }
}

public static class FileDbExtensions
{
    public static FileDto ToFileDto(this FileDb fileDb)
    {
        return new FileDto
        {
            DbId = fileDb.Id,
            ExternalId = fileDb.ExternalId,
            OwnerId = fileDb.OwnerId,
            FileName = fileDb.FileName,
            FileStatus = fileDb.FileStatus,
            FileType = fileDb.FileType,
            CreatedAt = fileDb.CreatedAt
        };
    }
}