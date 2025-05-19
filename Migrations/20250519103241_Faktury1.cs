using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class Faktury1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FakturaId",
                table: "ItemOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Faktury",
                columns: table => new
                {
                    FakturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faktury", x => x.FakturaId);
                    table.ForeignKey(
                        name: "FK_Faktury_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrders_FakturaId",
                table: "ItemOrders",
                column: "FakturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Faktury_UserId",
                table: "Faktury",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrders_Faktury_FakturaId",
                table: "ItemOrders",
                column: "FakturaId",
                principalTable: "Faktury",
                principalColumn: "FakturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrders_Faktury_FakturaId",
                table: "ItemOrders");

            migrationBuilder.DropTable(
                name: "Faktury");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrders_FakturaId",
                table: "ItemOrders");

            migrationBuilder.DropColumn(
                name: "FakturaId",
                table: "ItemOrders");
        }
    }
}
