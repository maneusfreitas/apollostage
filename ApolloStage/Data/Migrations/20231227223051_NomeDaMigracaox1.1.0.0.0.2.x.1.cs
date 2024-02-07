using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigracaox110002x1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mail",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }
    }
}
