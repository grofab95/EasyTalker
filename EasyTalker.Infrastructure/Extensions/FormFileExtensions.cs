using System.IO;
using Microsoft.AspNetCore.Http;

namespace EasyTalker.Infrastructure.Extensions;

public static class FormFileExtensions
{
    public static string GetExtension(this IFormFile formFile)
    {
        return Path.GetExtension(formFile.FileName);
    }
}