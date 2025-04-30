using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_buddys_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageReplyToMessageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageReplyedToMessageId",
                table: "CommunityChatModel",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageReplyedToMessageId",
                table: "CommunityChatModel");
        }
    }
}
