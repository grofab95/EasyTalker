using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Easy.MessageHub;
using EasyTalker.Api.Models;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Enums;
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
            var conversations = await _conversationStore.GetLoggedUserConversations(userId);
            return ApiResponse<ConversationDto[]>.Success(conversations);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto[]>.Failure(ex);
        }
    }
    
    [HttpPost]
    [Route("messages")]
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
    
    [HttpGet]
    [Route("{conversationId:long}/messages")]
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
    
    [HttpPut]
    [Route("{conversationId:long}/participants/add")]
    public async Task<ApiResponse<ConversationDto>> AddUsersToConversation(
        [FromRoute] long conversationId,
        [FromBody] string[] userIds)
    {
        try
        {
            await _conversationStore.AddUsersToConversation(
                conversationId,
                userIds);
            
            var updatedConversation = await NotifyConversationUpdated(
                conversationId);
            
            return ApiResponse<ConversationDto>.Success(updatedConversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex);
        }
    }
    
    [HttpPut]
    [Route("{conversationId:long}/participants/{participantId}/update-access-status")]
    public async Task<ApiResponse> UpdateUserConversationAccessStatus([FromRoute] long conversationId, [FromRoute] string participantId, [FromBody] ConversationAccessStatus conversationAccessStatus)
    {
        try
        {
            await _conversationStore.UpdateUserConversationAccessStatus(conversationId, participantId, conversationAccessStatus);
            await NotifyConversationUpdated(conversationId);
            
            return ApiResponse.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse.Failure(ex);
        }
    }

    [HttpPatch]
    [Route("{conversationId:long}")]
    public async Task<ApiResponse<ConversationLastSeenDto>> UpdateConversationLastSeenAt([FromRoute] long conversationId)
    {
        try
        {
            var conversationLastSeenDto = await _conversationStore.UpdateLoggedUserConversationLastSeenAt(conversationId, LoggedUserId);
            return ApiResponse<ConversationLastSeenDto>.Success(conversationLastSeenDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationLastSeenDto>.Failure(ex);
        }
    }
    
    [HttpPut]
    [Route("{conversationId:long}/update-status")]
    public async Task<ApiResponse> UpdateConversationStatus([FromRoute] long conversationId, [FromBody] ConversationStatus status)
    {
        try
        {
            await _conversationStore.UpdateConversationStatus(conversationId, status);
            await NotifyConversationUpdated(conversationId);
            
            return ApiResponse.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse.Failure(ex);
        }
    }

    private async Task<ConversationDto> NotifyConversationUpdated(long conversationsId)
    {
        var conversation = (await _conversationStore.GetConversations(new []{ conversationsId }, LoggedUserId)).First();
        
        _messageHub.Publish(new ConversationUpdated(conversation));
        return conversation;
    }
    
    private string LoggedUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}