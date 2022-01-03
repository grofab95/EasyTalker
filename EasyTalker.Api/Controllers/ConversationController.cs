using System;
using System.Threading.Tasks;
using EasyTalker.Api.Models;
using EasyTalker.Core.Adapters;
using EasyTalker.Infrastructure.Dto.Conversation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationStore _conversationStore;

    public ConversationController(IConversationStore conversationStore)
    {
        _conversationStore = conversationStore;
    }
    
    [HttpGet("{userId}")]
    public async Task<ApiResponse<ConversationDto[]>> GetUserConversations([FromRoute] string userId)
    {
        try
        {
            var conversations = await _conversationStore.GetUserConversations(userId);
            return ApiResponse<ConversationDto[]>.Success(conversations);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto[]>.Failure(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ApiResponse<ConversationDto>> Create([FromBody] ConversationCreateDto conversationCreateDto)
    {
        try
        {
            var conversation = await _conversationStore.Add(conversationCreateDto);
            return ApiResponse<ConversationDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex.Message);
        }
    }
}