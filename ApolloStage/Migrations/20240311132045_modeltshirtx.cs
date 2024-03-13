using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class modeltshirtx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Images",
                table: "Tshirt",
                newName: "Images4");

            migrationBuilder.AddColumn<string>(
                name: "Images1",
                table: "Tshirt",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Images2",
                table: "Tshirt",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Images3",
                table: "Tshirt",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images1",
                table: "Tshirt");

            migrationBuilder.DropColumn(
                name: "Images2",
                table: "Tshirt");

            migrationBuilder.DropColumn(
                name: "Images3",
                table: "Tshirt");

            migrationBuilder.RenameColumn(
                name: "Images4",
                table: "Tshirt",
                newName: "Images");
        }
    }
}
