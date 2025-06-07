using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class poprawabony1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bony",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Bony_UserId",
                table: "Bony",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bony_AspNetUsers_UserId",
                table: "Bony",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bony_AspNetUsers_UserId",
                table: "Bony");

            migrationBuilder.DropIndex(
                name: "IX_Bony_UserId",
                table: "Bony");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bony");
        }
    }
}
