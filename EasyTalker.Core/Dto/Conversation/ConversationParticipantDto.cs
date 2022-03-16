using EasyTalker.Core.Enums;

namespace EasyTalker.Core.Dto.Conversation;

public record ConversationParticipantDto(string Id, ConversationAccessStatus AccessStatus);