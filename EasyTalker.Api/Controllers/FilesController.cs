using System;
using System.Threading.Tasks;
using EasyTalker.Api.Extensions;
using EasyTalker.Api.Models;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class FilesController : Controller
{
    private readonly FilePersistenceManager _filePersistenceManager;

    public FilesController(FilePersistenceManager filePersistenceManager)
    {
        _filePersistenceManager = filePersistenceManager;
    }

    //[Route("upload")]
    [HttpPost]
    [DisableRequestSizeLimit]
    //[RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<ApiResponse<FileDto>> UploadFile([FromForm] UploadFileDto uploadFileDto)
    {
        try
        {
            var fileDto = await _filePersistenceManager.UploadFile(User.GetId(), uploadFileDto);
            return ApiResponse<FileDto>.Success(fileDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<FileDto>.Failure(ex);
        }
    }

    [Route("external-id/{externalId}")]
    [HttpGet]
    public async Task<ApiResponse<FileDto[]>> GetFiles([FromRoute] string externalId)
    {
        try
        {
            var filesDto = await _filePersistenceManager.GetFilesInfoByExternalId(externalId);
            return ApiResponse<FileDto[]>.Success(filesDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<FileDto[]>.Failure(ex);
        }
    }
}