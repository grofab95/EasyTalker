using System;
using System.Linq;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Views;

namespace EasyTalker.Database.Extensions;

public static class ViewsExtensions
{
    public static ConversationDto ToConversationDto(this ConversationInfosView view, DateTime lastSeenAt)
    {
        var participants = view.ConversationParticipantsString
            .Split('|')
            .Select(CreateConversationParticipantDto)
            .ToArray();

        return new ConversationDto
        {
            Id = view.ConversationId,
            Title = view.Title,
            Status = Enum.Parse<ConversationStatus>(view.Status),
            CreatorId = view.CreatorId,
            LastSeenAt = lastSeenAt,
            Participants = participants
        };
    }

    private static ConversationParticipantDto CreateConversationParticipantDto(string joinedString)
    {
        var split = joinedString.Split(':');
        var participantId = split[0].Trim();
        var accessStatus = Enum.Parse<ConversationAccessStatus>(split[1]);
        return new ConversationParticipantDto(participantId, accessStatus);
    }
}