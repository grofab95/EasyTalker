using EasyTalker.Core.Enums;

namespace EasyTalker.Database.Entities;

public class FileDb : EntityDb
{
    public string ExternalId { get; set; }
    public string FileName { get; set; }
    public string OwnerId { get; set; }
    public FileStatus FileStatus { get; set; }
    public FileType FileType { get; set; }
}