using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalker.Database.Migrations
{
    public partial class addSenderIdToconversation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersConversations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversations_ConversationId_UserId",
                table: "UsersConversations",
                columns: new[] { "ConversationId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_CreatorId_Title",
                table: "Conversations",
                columns: new[] { "CreatorId", "Title" },
                unique: true,
                filter: "[CreatorId] IS NOT NULL AND [Title] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersConversations_ConversationId_UserId",
                table: "UsersConversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_CreatorId_Title",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Conversations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersConversations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
