using System.Linq;
using Microsoft.AspNetCore.Http;

namespace EasyTalker.Core.Extensions;

public static class FormFileExtensions
{
    public static string GetExtension(this IFormFile file)
    {
        return file.Name.Split(".").LastOrDefault();
    }
}