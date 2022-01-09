using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalker.Database.Migrations
{
    public partial class addHasAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAccess",
                table: "UsersConversations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAccess",
                table: "UsersConversations");
        }
    }
}
