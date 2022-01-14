using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalker.Database.Migrations
{
    public partial class externalidAsString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_ExternalId_FileName",
                table: "Files");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Files",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ExternalId_FileName",
                table: "Files",
                columns: new[] { "ExternalId", "FileName" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [FileName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_ExternalId_FileName",
                table: "Files");

            migrationBuilder.AlterColumn<long>(
                name: "ExternalId",
                table: "Files",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ExternalId_FileName",
                table: "Files",
                columns: new[] { "ExternalId", "FileName" },
                unique: true,
                filter: "[FileName] IS NOT NULL");
        }
    }
}
