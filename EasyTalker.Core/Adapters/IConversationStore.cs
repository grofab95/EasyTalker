using System.Threading.Tasks;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Adapters;

public interface IConversationStore
{
    Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto);
    Task<ConversationDto[]> GetUserConversations(string loggedUserId);
    Task<MessageDto[]> GetMessages(long conversationId);
    Task<ConversationDto> AddParticipant(long conversationId, string[] userIds, string loggedUserId);
    Task<ConversationDto> RemoveParticipant(long conversationId, string[] participantsIds, string loggedUserId);
    Task<ConversationLastSeenDto> UpdateConversationLastSeenAt(long conversationId, string loggedUserId);
}