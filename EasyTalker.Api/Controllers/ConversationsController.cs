using System;
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
            var conversations = await _conversationStore.GetUserConversations(userId);
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
    public async Task<ApiResponse<ConversationDto>> AddParticipants([FromRoute] long conversationId,
        [FromBody] string[] usersId)
    {
        try
        {
            var conversation = await _conversationStore.AddParticipant(conversationId, usersId, GetLoggedUserId);
            _messageHub.Publish(new ConversationUpdated(conversation));
            return ApiResponse<ConversationDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex);
        }
    }
    
    [HttpPut]
    [Route("{conversationId:long}/participants/remove")]
    public async Task<ApiResponse<ConversationDto>> RemoveParticipants([FromRoute] long conversationId,
        [FromBody] string[] participantsIds)
    {
        try
        {
            var conversation = await _conversationStore.RemoveParticipant(conversationId, participantsIds, GetLoggedUserId);
            _messageHub.Publish(new ConversationUpdated(conversation));
            return ApiResponse<ConversationDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            return ApiResponse<ConversationDto>.Failure(ex);
        }
    }

    [HttpPatch]
    [Route("{conversationId:long}")]
    public async Task<ApiResponse<ConversationLastSeenDto>> UpdateConversationLastSeenAt([FromRoute] long conversationId)
    {
        try
        {
            var conversationLastSeenDto = await _conversationStore.UpdateConversationLastSeenAt(conversationId, GetLoggedUserId);
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
            var conversation = await _conversationStore.UpdateConversationStatus(conversationId, status, GetLoggedUserId);
            _messageHub.Publish(new ConversationUpdated(conversation));
            
            return ApiResponse.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse.Failure(ex);
        }
    }
    
    private string GetLoggedUserId =>  HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
}