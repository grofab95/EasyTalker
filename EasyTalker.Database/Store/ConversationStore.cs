using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Enums;

namespace EasyTalker.Database.Store;

public class ConversationStore : IConversationStore
{
    public ConversationStore()
    {
            
    }
    
    public Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<ConversationDto[]> GetLoggedUserConversations(string loggedUserId)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDto[]> GetMessages(long conversationId)
    {
        throw new NotImplementedException();
    }

    public Task AddUsersToConversation(long conversationId, IEnumerable<string> userIds)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserConversationAccessStatus(long conversationId, string userId, ConversationAccessStatus accessStatus)
    {
        throw new NotImplementedException();
    }

    public Task<ConversationLastSeenDto> UpdateLoggedUserConversationLastSeenAt(long conversationId, string loggedUserId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateConversationStatus(long conversationId, ConversationStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<ConversationDto[]> GetConversations(long[] conversationsIds, string loggedUserId)
    {
        throw new NotImplementedException();
    }
}