using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class tshirt_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Association",
                table: "Tshirt",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Tshirt",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Association",
                table: "Tshirt");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Tshirt");
        }
    }
}
