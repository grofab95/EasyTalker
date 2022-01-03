using System.Threading.Tasks;
using EasyTalker.Infrastructure.Dto.Conversation;

namespace EasyTalker.Core.Adapters;

public interface IConversationStore
{
    Task<ConversationDto[]> GetUserConversations(string userId);
    Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto);
}