using System;
using Microsoft.AspNetCore.Http;

namespace EasyTalker.Core.Dto.File;

public class UploadFileDto
{
    public Guid UploadId { get; set; }
    public string ExternalId { get; set; }
    public IFormFile File { get; set; }
}