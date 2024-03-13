using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Migrations
{
    /// <inheritdoc />
    public partial class ProductOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserMail = table.Column<string>(type: "TEXT", nullable: false),
                    TshirtTitle = table.Column<string>(type: "TEXT", nullable: false),
                    TshirtSize = table.Column<string>(type: "TEXT", nullable: false),
                    TshirtColor = table.Column<string>(type: "TEXT", nullable: false),
                    TshirtCount = table.Column<string>(type: "TEXT", nullable: false),
                    TshirtPrice = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOrder", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOrder");
        }
    }
}
