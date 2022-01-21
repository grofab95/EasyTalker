using System;
using System.Linq;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Database.Entities;
using EasyTalker.Database.Views;

namespace EasyTalker.Database.Extensions;

public static class ViewsExtesnions
{
    public static ConversationDto ToConversationDto(this ConversationInfosView view, MessageDto lastMessage, DateTime lastSeenAt)
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
            LastSeenAt = lastSeenAt,
            //LastMessageAt = view.LastMessageAt ?? view.CreatedAt,
            Participants = participants,
            LastMessage = lastMessage
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