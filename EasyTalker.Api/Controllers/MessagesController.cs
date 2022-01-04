using System;
using System.Threading.Tasks;
using EasyTalker.Api.Models;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageStore _messageStore;

    public MessagesController(IMessageStore messageStore)
    {
        _messageStore = messageStore;
    }
    
    // [HttpPost]
    // public async Task<ApiResponse<MessageDto>> Add([FromBody] MessageCreateDto messageCreateDto)
    // {
    //     try
    //     {
    //         var result = await _messageStore.Add(messageCreateDto);
    //         return ApiResponse<MessageDto>.Success(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return ApiResponse<MessageDto>.Failure(ex.Message);
    //     }
    // }
    // //messages/1/messages
    // [Route("{conversationId:long}/messages")]
    // [HttpGet]
    // public async Task<ApiResponse<MessageDto[]>> GetMessages([FromRoute] long conversationId)
    // {
    //     try
    //     {
    //         var result = await _conversationStore.GetMessages(conversationId);
    //         return ApiResponse<MessageDto[]>.Success(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return ApiResponse<MessageDto[]>.Failure(ex.Message);
    //     }
    // }
}