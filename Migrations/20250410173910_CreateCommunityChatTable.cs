using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_buddys_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class CreateCommunityChatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityChatModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdSender = table.Column<int>(type: "int", nullable: false),
                    UserSenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    CommunityModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityChatModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityChatModel_Communitys_CommunityModelId",
                        column: x => x.CommunityModelId,
                        principalTable: "Communitys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityChatModel_CommunityModelId",
                table: "CommunityChatModel",
                column: "CommunityModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityChatModel");
        }
    }
}
