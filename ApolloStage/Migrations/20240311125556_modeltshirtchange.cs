using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class modeltshirtchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Tshirt");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Tshirt",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Tshirt",
                newName: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "Tshirt",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Color",
                table: "Tshirt",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TshirtCount",
                table: "ProductOrder",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tshirt",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Images",
                table: "Tshirt",
                newName: "Image");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "Tshirt",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Tshirt",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Tshirt",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TshirtCount",
                table: "ProductOrder",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
