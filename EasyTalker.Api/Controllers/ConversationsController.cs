using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Easy.MessageHub;
using EasyTalker.Api.Models;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationStore _conversationStore;
    private readonly IMessageStore _messageStore;
    private readonly IMessageHub _messageHub;

    public ConversationsController(IConversationStore conversationStore, IMessageStore messageStore, IMessageHub messageHub)
    {
        _conversationStore = conversationStore;
        _messageStore = messageStore;
        _messageHub = messageHub;
    }
    
    [HttpPost]
    public async Task<ApiResponse<ConversationDto>> Create([FromBody] ConversationCreateDto conversationCreateDto)
    {
        try
        {
            var conversation = await _conversationStore.Add(conversationCreateDto);
            
            _messageHub.Publish(new ConversationCreated(conversation));
            
            return ApiResponse<ConversationDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex);
        }
    }
    
    [HttpGet("users/{userId}")]
    public async Task<ApiResponse<ConversationDto[]>> GetUserConversations([FromRoute] string userId)
    {
        try
        {
            var conversations = await _conversationStore.GetUserConversations(userId);
            return ApiResponse<ConversationDto[]>.Success(conversations);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto[]>.Failure(ex);
        }
    }
    
    [Route("messages")]
    [HttpPost]
    public async Task<ApiResponse<MessageDto>> AddMessage([FromBody] MessageCreateDto messageCreateDto)
    {
        try
        {
            var createdMessage = await _messageStore.Add(messageCreateDto);
            
            _messageHub.Publish(new MessageCreated(createdMessage));

            return ApiResponse<MessageDto>.Success(createdMessage);
        }
        catch (Exception ex)
        {
            return ApiResponse<MessageDto>.Failure(ex);
        }
    }
    
    [Route("{conversationId:long}/messages")]
    [HttpGet]
    public async Task<ApiResponse<MessageDto[]>> GetMessages([FromRoute] long conversationId)
    {
        try
        {
            var messages = await _conversationStore.GetMessages(conversationId);
            
            return ApiResponse<MessageDto[]>.Success(messages);
        }
        catch (Exception ex)
        {
            return ApiResponse<MessageDto[]>.Failure(ex);
        }
    }

    [Route("{conversationId:long}/participants/add")]
    [HttpPut]
    public async Task<ApiResponse<ConversationDto>> AddParticipants([FromRoute] long conversationId,
        [FromBody] string[] usersId)
    {
        try
        {
            var conversation = await _conversationStore.AddParticipant(conversationId, usersId);
            
            _messageHub.Publish(new ConversationUpdated(conversation));
            
            return ApiResponse<ConversationDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex);
        }
    }
    
    [Route("{conversationId:long}/participants/remove")]
    [HttpPut]
    public async Task<ApiResponse<ConversationDto>> RemoveParticipants([FromRoute] long conversationId,
        [FromBody] string[] participantsIds)
    {
        try
        {
            var conversation = await _conversationStore.RemoveParticipant(conversationId, participantsIds);
            
            _messageHub.Publish(new ConversationUpdated(conversation));
            
            return ApiResponse<ConversationDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex);
        }
    }

    [Route("{conversationId:long}")]
    [HttpPatch]
    public async Task<ApiResponse> UpdateConversationLastSeenAt([FromRoute] long conversationId, [FromBody] DateTime seenAt)
    {
        try
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            
            await _conversationStore.UpdateConversationLastSeenAt(conversationId, userId, seenAt);
            
            return ApiResponse.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse.Failure(ex);
        }
    }
}