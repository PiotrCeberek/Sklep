using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class poprawieniepowiadomiendlapracownikow2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrders_DeletedProducts_DeletedProductProductId",
                table: "ItemOrders");

            migrationBuilder.RenameColumn(
                name: "DeletedProductProductId",
                table: "ItemOrders",
                newName: "DeletedProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemOrders_DeletedProductProductId",
                table: "ItemOrders",
                newName: "IX_ItemOrders_DeletedProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrders_DeletedProducts_DeletedProductId",
                table: "ItemOrders",
                column: "DeletedProductId",
                principalTable: "DeletedProducts",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrders_DeletedProducts_DeletedProductId",
                table: "ItemOrders");

            migrationBuilder.RenameColumn(
                name: "DeletedProductId",
                table: "ItemOrders",
                newName: "DeletedProductProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemOrders_DeletedProductId",
                table: "ItemOrders",
                newName: "IX_ItemOrders_DeletedProductProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrders_DeletedProducts_DeletedProductProductId",
                table: "ItemOrders",
                column: "DeletedProductProductId",
                principalTable: "DeletedProducts",
                principalColumn: "ProductId");
        }
    }
}
