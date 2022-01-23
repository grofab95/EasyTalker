﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalker.Database.Migrations
{
    public partial class AddViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[ConversationInfosView]
                AS
                WITH ConversationInfos AS (SELECT        ConversationId, STRING_AGG(UserId + ':' + STR(HasAccess), '|') AS ConversationParticipants
                                                                              FROM            dbo.UsersConversations AS uc
                                                                              GROUP BY ConversationId)
                    SELECT        Id AS ConversationId, Title, CreatorId, CreatedAt,
                                                  (SELECT        MAX(CreatedAt) AS Expr1
                                                    FROM            dbo.Messages AS m
                                                    WHERE        (ConversationId = c.Id)) AS LastMessageAt,
                                                  (SELECT        ConversationParticipants
                                                    FROM            ConversationInfos AS ConversationInfos_1
                                                    WHERE        (ConversationId = c.Id)) AS ConversationParticipantsString,
                                                  (SELECT        TOP (1) Id
                                                    FROM            dbo.Messages
                                                    WHERE        (ConversationId = c.Id)
                                                    ORDER BY CreatedAt DESC) AS LastMessageId
                     FROM            dbo.Conversations AS c
                GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
