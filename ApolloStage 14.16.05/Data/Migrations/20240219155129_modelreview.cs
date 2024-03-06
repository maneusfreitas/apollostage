using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class modelreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlbumReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserMail = table.Column<string>(type: "TEXT", nullable: false),
                    AlbumId = table.Column<string>(type: "TEXT", nullable: false),
                    reviewTitle = table.Column<string>(type: "TEXT", nullable: false),
                    reviewDescription = table.Column<string>(type: "TEXT", nullable: false),
                    reviewRecommendation = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumReview", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumReview");
        }
    }
}
