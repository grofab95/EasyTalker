using System.Linq;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Database.Views;

namespace EasyTalker.Database.Extensions;

public static class ViewsExtesnions
{
    public static ConversationDto ToConversationDto(this ConversationInfosView view)
    {
        var participants = view.ConversationParticipantsString
            .Split('|')
            .Select(CreateConversationParticipantDto)
            .ToArray();

        return new ConversationDto
        {
            Id = view.ConversationId,
            Title = view.Title,
            CreatorId = view.CreatorId,
            Participants = participants
        };
    }

    private static ConversationParticipantDto CreateConversationParticipantDto(string joinedString)
    {
        var splitted = joinedString.Split(':');
        var participantId = splitted[0].Trim();
        var hasAccess = splitted[1].Trim() == "1";
        
        return new ConversationParticipantDto(participantId, hasAccess);
    }
}