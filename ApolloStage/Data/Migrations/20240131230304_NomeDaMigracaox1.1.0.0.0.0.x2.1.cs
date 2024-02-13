using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigracaox110000x21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlbumRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userEmail = table.Column<string>(type: "TEXT", nullable: false),
                    albumId = table.Column<string>(type: "TEXT", nullable: false),
                    starRating = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumRatings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumRatings");
        }
    }
}
