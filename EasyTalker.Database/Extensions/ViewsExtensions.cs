﻿using System;
using System.Linq;
using EasyTalker.Core.Dto.Conversation;
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
            CreatorId = view.CreatorId,
            LastSeenAt = lastSeenAt,
            Participants = participants
        };
    }

    private static ConversationParticipantDto CreateConversationParticipantDto(string joinedString)
    {
        var split = joinedString.Split(':');
        var participantId = split[0].Trim();
        var hasAccess = split[1].Trim() == "1";
        return new ConversationParticipantDto(participantId, hasAccess);
    }
}