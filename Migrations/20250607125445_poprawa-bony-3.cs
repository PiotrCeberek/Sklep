using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class poprawabony3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bony_AspNetUsers_UserId",
                table: "Bony");

            migrationBuilder.DropIndex(
                name: "IX_Bony_UserId",
                table: "Bony");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Bony",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Bony",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
    }
}
