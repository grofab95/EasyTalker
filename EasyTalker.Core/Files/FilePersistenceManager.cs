using System.IO;
using System.Threading.Tasks;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Configuration;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EasyTalker.Core.Files;

public class FilePersistenceManager
{
    private readonly IFileStore _fileStore;
    private readonly PathsOptions _pathsOptions;

    public FilePersistenceManager(IFileStore fileStore, IOptions<PathsOptions> pathsOptions)
    {
        _fileStore = fileStore;
        _pathsOptions = pathsOptions.Value;
    }

    public async Task<FileDto> UploadFile(string ownerId, UploadFileDto uploadFileDto)
    {
        var fileDto = await _fileStore.SaveFileInfo(ownerId, uploadFileDto);
        await SaveOnDisc(uploadFileDto.File, fileDto);
        return await SetUploadedStatus(fileDto.DbId);
    }

    public async Task<FileDto[]> GetFilesInfoByExternalId(string externalId)
    {
        return await _fileStore.GetFilesInfoByExternalIds(new[] {externalId});
    }

    private async Task SaveOnDisc(IFormFile file, FileDto fileDto)
    {
        var directoryPath = GetDirectoryPath(fileDto);
        if (string.IsNullOrEmpty(directoryPath))
            throw new DirectoryNotFoundException();
        
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        
        await using var fileStream = new FileStream(GetFilePath(fileDto), FileMode.Create, FileAccess.Write);
        await file.CopyToAsync(fileStream);
    }

    private string GetFilePath(FileDto fileDto)
    {
        return Path.Combine(_pathsOptions.UploadFilesPath, fileDto.ExternalId, fileDto.FileName);
    }
    
    private string GetDirectoryPath(FileDto fileDto)
    {
        return Path.Combine(_pathsOptions.UploadFilesPath, fileDto.ExternalId);
    }

    private async Task<FileDto> SetUploadedStatus(long dbId)
    {
        return await _fileStore.UpdateFileStatus(dbId, FileStatus.Uploaded);
    }
}