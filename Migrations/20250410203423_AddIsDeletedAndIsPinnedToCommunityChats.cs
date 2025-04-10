using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_buddys_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedAndIsPinnedToCommunityChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CommunityChatModel",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "CommunityChatModel",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CommunityChatModel");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "CommunityChatModel");
        }
    }
}
