using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApolloStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigracaox11x000x1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteAlbum_AspNetUsers_UserId1",
                table: "FavoriteAlbum");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteAlbum_UserId1",
                table: "FavoriteAlbum");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "FavoriteAlbum");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FavoriteAlbum",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "UserMail",
                table: "FavoriteAlbum",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteAlbum_UserId",
                table: "FavoriteAlbum",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteAlbum_AspNetUsers_UserId",
                table: "FavoriteAlbum",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteAlbum_AspNetUsers_UserId",
                table: "FavoriteAlbum");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteAlbum_UserId",
                table: "FavoriteAlbum");

            migrationBuilder.DropColumn(
                name: "UserMail",
                table: "FavoriteAlbum");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "FavoriteAlbum",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "FavoriteAlbum",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteAlbum_UserId1",
                table: "FavoriteAlbum",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteAlbum_AspNetUsers_UserId1",
                table: "FavoriteAlbum",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
