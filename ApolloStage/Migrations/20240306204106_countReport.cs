using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class countReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "count",
                table: "ReviewReports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "count",
                table: "ReviewReports");
        }
    }
}
