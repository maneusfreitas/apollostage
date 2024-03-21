using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class updateproductorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "state",
                table: "ProductOrder",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "pointstoapply",
                table: "ProductOrder",
                newName: "Pointstoapply");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "ProductOrder",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Pointstoapply",
                table: "ProductOrder",
                newName: "pointstoapply");
        }
    }
}
