using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class addtshirtchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Tshirt");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Tshirt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Tshirt",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Tshirt",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
