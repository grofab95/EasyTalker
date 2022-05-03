using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Enums;
using EasyTalker.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace EasyTalker.Database.Store;

public class FileStore : IFileStore
{
    private readonly IDbConnection _dbConnection;

    public FileStore(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<FileDto> SaveFileInfo(string ownerId, UploadFileDto uploadFileDto)
    {
        // if (await IsFileExist(dbContext, uploadFileDto))
        //     throw new Exception("Uploading file is already exist in this conversation");
        
        var fileType = DetermineFileType(uploadFileDto.File);
        var savedFileId = await _dbConnection.QuerySingleAsync<long>(@"
            DECLARE @InsertedRows AS TABLE (Id bigint);
            INSERT INTO Files (ExternalId, FileName, OwnerId, FileStatus, FileType) OUTPUT Inserted.Id INTO @InsertedRows
            VALUES (@externalId, @fileName, @ownerId, @fileStatus, @fileType);
            SELECT Id FROM @InsertedRows", new
        {
            externalId = uploadFileDto.ExternalId,
            fileName = uploadFileDto.File.FileName,
            ownerId,
            fileStatus = FileStatus.Saved.ToString(),
            fileType
        });
        
        var savedFile =  await _dbConnection.QueryFirstOrDefaultAsync<FileDto>(@"
            SELECT Id, ExternalId, OwnerId, FileName, FileStatus, FileType, CreatedAt
            FROM Files
            WHERE Id = @id", new
        {
            id = savedFileId
        });

        savedFile.DbId = savedFileId;
        return savedFile;
    }

    // private async Task<bool> IsFileExist(EasyTalkerContext dbContext, UploadFileDto uploadFileDto)
    // {
    //     return await dbContext.Files.AnyAsync(x => x.ExternalId == uploadFileDto.ExternalId && x.FileName == uploadFileDto.File.FileName);
    // }

    public async Task<FileDto[]> GetFilesInfoByExternalIds(string[] externalIds)
    {
        return (await _dbConnection.QueryAsync<FileDto>(@"
            SELECT Id, ExternalId, OwnerId, FileName, FileStatus, FileType, CreatedAt
            FROM Files
            WHERE ExternalId In @externalIds", new
        {
            externalIds
        })).ToArray();
    }

    public async Task<FileDto> UpdateFileStatus(long dbId, FileStatus fileStatus)
    {
        await _dbConnection.QueryAsync(@"
            UPDATE Files
            SET FileStatus = @fileStatus
            WHERE Id = @id", new
        {
            id = dbId,
            fileStatus = fileStatus.ToString()
        });

        var updatedFile = await _dbConnection.QueryFirstOrDefaultAsync<FileDto>(@"
            SELECT Id, ExternalId, OwnerId, FileName, FileStatus, FileType, CreatedAt
            FROM Files
            WHERE Id = @id", new
        {
            id = dbId
        });

        updatedFile.DbId = dbId;
        return updatedFile;
    }

    private FileType DetermineFileType(IFormFile file)
    {
        var imagesExtensions = new[] {"jpg", "jpeg", "gif", "png", "bmp"};
        var extension = file.GetExtension()?.Replace(".", string.Empty);

        return imagesExtensions.Contains(extension?.ToLower().Trim())
            ? FileType.Image
            : FileType.Other;
    }
}