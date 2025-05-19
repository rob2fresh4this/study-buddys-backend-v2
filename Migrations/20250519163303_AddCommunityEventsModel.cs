using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_buddys_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityEventsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    CurrentParticipants = table.Column<int>(type: "int", nullable: false),
                    EventIsPublic = table.Column<bool>(type: "bit", nullable: false),
                    EventIsCancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventOrganizerDTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityEventsModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventOrganizerDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventOrganizerDTO_CommunityEvents_CommunityEventsModelId",
                        column: x => x.CommunityEventsModelId,
                        principalTable: "CommunityEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventParticipantDTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityEventsModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipantDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipantDTO_CommunityEvents_CommunityEventsModelId",
                        column: x => x.CommunityEventsModelId,
                        principalTable: "CommunityEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventOrganizerDTO_CommunityEventsModelId",
                table: "EventOrganizerDTO",
                column: "CommunityEventsModelId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantDTO_CommunityEventsModelId",
                table: "EventParticipantDTO",
                column: "CommunityEventsModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventOrganizerDTO");

            migrationBuilder.DropTable(
                name: "EventParticipantDTO");

            migrationBuilder.DropTable(
                name: "CommunityEvents");
        }
    }
}
