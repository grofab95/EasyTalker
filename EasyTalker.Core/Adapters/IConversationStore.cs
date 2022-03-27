using System.Collections.Generic;
using System.Threading.Tasks;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Enums;

namespace EasyTalker.Core.Adapters;

public interface IConversationStore
{
    Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto);
    Task<ConversationDto[]> GetLoggedUserConversations(string loggedUserId);
    Task<MessageDto[]> GetMessages(long conversationId);
    Task AddUsersToConversation(long conversationId, IEnumerable<string> userIds);
    Task UpdateUserConversationAccessStatus(long conversationId, string userId, ConversationAccessStatus accessStatus);
    Task<ConversationLastSeenDto> UpdateLoggedUserConversationLastSeenAt(long conversationId, string loggedUserId);
    Task UpdateConversationStatus(long conversationId, ConversationStatus status);
    Task<ConversationDto[]> GetConversations(long[] conversationsIds, string loggedUserId);
}