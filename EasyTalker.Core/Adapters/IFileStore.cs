using System.Threading.Tasks;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Enums;

namespace EasyTalker.Core.Adapters;

public interface IFileStore
{
    public Task<FileDto> SaveFileInfo(string ownerId, UploadFileDto uploadFileDto);
    public Task<FileDto[]> GetFilesInfoByExternalIds(string[] externalIds);
    public Task<FileDto> UpdateFileStatus(long dbId, FileStatus fileStatus);
}