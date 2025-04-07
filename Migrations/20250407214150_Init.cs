using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_buddys_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communitys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommunityOwnerID = table.Column<int>(type: "int", nullable: false),
                    IsCommunityOwner = table.Column<bool>(type: "bit", nullable: false),
                    CommunityIsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CommunityIsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CommunityOwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunitySubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityMemberCount = table.Column<int>(type: "int", nullable: false),
                    CommunityRequests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityDifficulty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communitys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnedCommunitys = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinedCommunitys = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityRequests = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityMemberModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunityModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityMemberModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityMemberModel_Communitys_CommunityModelId",
                        column: x => x.CommunityModelId,
                        principalTable: "Communitys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberModel_CommunityModelId",
                table: "CommunityMemberModel",
                column: "CommunityModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityMemberModel");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Communitys");
        }
    }
}
