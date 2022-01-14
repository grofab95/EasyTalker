using EasyTalker.Core.Enums;

namespace EasyTalker.Core.Dto.File;

public class FileDto
{
    public long DbId { get; set; }
    public string ExternalId { get; set; }
    public string FileName { get; set; }
    public FileStatus FileStatus { get; set; }
}