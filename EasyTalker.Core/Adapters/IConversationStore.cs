using System.Threading.Tasks;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Adapters;

public interface IConversationStore
{
    Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto);
    Task<ConversationDto[]> GetUserConversations(string userId);
    Task<MessageDto[]> GetMessages(long conversationId);
    Task AddParticipant(long conversationId, string[] userIds);
    Task RemoveParticipant(long conversationId, string[] userIds);
}