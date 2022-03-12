using System;
using System.Threading.Tasks;
using Easy.MessageHub;
using EasyTalker.Api.Extensions;
using EasyTalker.Api.Models;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Events;
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
    private readonly IMessageHub _messageHub;

    public FilesController(FilePersistenceManager filePersistenceManager, IMessageHub messageHub)
    {
        _filePersistenceManager = filePersistenceManager;
        _messageHub = messageHub;
    }

    [HttpPost]
    [DisableRequestSizeLimit, RequestFormLimits(
         MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
    public async Task<ApiResponse<FileDto>> UploadFile([FromForm] UploadFileDto uploadFileDto)
    {
        try
        {
            var fileDto = await _filePersistenceManager.UploadFile(User.GetId(), uploadFileDto);
            _messageHub.Publish(new FileUploaded(fileDto));
            return ApiResponse<FileDto>.Success(fileDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<FileDto>.Failure(ex);
        }
    }

    [HttpGet]
    [Route("external-id/{externalId}")]
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