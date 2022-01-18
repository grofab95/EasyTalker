using System;
using System.Linq;
using System.Threading.Tasks;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Store;

public class FileStore : IFileStore
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FileStore(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<FileDto> SaveFileInfo(string ownerId, UploadFileDto uploadFileDto)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        if (await IsFileExist(dbContext, uploadFileDto))
            throw new Exception("Uploading file is already exist in this conversation");
        
        var savedFile = await dbContext.Files.AddAsync(new FileDb(ownerId, uploadFileDto));
        await dbContext.SaveChangesAsync();
        
        return savedFile.Entity.ToFileDto();
    }

    private async Task<bool> IsFileExist(EasyTalkerContext dbContext, UploadFileDto uploadFileDto)
    {
        return await dbContext.Files.AnyAsync(x => x.ExternalId == uploadFileDto.ExternalId && x.FileName == uploadFileDto.File.FileName);
    }

    public async Task<FileDto[]> GetFilesInfoByExternalIds(string[] externalIds)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        return await dbContext.Files
            .Where(x => externalIds.Contains(x.ExternalId))
            .Select(x => x.ToFileDto())
            .ToArrayAsync();
    }

    public async Task<FileDto> UpdateFileStatus(long dbId, FileStatus fileStatus)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var fileDb = await dbContext.Files.FindAsync(dbId)
            ?? throw new Exception($"File with id {dbId} not found");

        fileDb.FileStatus = fileStatus;
        await dbContext.SaveChangesAsync();
        
        return fileDb.ToFileDto();
    }
}