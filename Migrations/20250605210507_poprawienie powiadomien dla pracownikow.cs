using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class poprawieniepowiadomiendlapracownikow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeletedProductProductId",
                table: "Promotions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedProductProductId",
                table: "ItemOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedProductProductId",
                table: "Favorites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedProductProductId",
                table: "ArticleComments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeletedProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedProducts", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_DeletedProducts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_DeletedProductProductId",
                table: "Promotions",
                column: "DeletedProductProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrders_DeletedProductProductId",
                table: "ItemOrders",
                column: "DeletedProductProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_DeletedProductProductId",
                table: "Favorites",
                column: "DeletedProductProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComments_DeletedProductProductId",
                table: "ArticleComments",
                column: "DeletedProductProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedProducts_CategoryId",
                table: "DeletedProducts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleComments_DeletedProducts_DeletedProductProductId",
                table: "ArticleComments",
                column: "DeletedProductProductId",
                principalTable: "DeletedProducts",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_DeletedProducts_DeletedProductProductId",
                table: "Favorites",
                column: "DeletedProductProductId",
                principalTable: "DeletedProducts",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrders_DeletedProducts_DeletedProductProductId",
                table: "ItemOrders",
                column: "DeletedProductProductId",
                principalTable: "DeletedProducts",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_DeletedProducts_DeletedProductProductId",
                table: "Promotions",
                column: "DeletedProductProductId",
                principalTable: "DeletedProducts",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleComments_DeletedProducts_DeletedProductProductId",
                table: "ArticleComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_DeletedProducts_DeletedProductProductId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrders_DeletedProducts_DeletedProductProductId",
                table: "ItemOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_DeletedProducts_DeletedProductProductId",
                table: "Promotions");

            migrationBuilder.DropTable(
                name: "DeletedProducts");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_DeletedProductProductId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrders_DeletedProductProductId",
                table: "ItemOrders");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_DeletedProductProductId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_ArticleComments_DeletedProductProductId",
                table: "ArticleComments");

            migrationBuilder.DropColumn(
                name: "DeletedProductProductId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DeletedProductProductId",
                table: "ItemOrders");

            migrationBuilder.DropColumn(
                name: "DeletedProductProductId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "DeletedProductProductId",
                table: "ArticleComments");
        }
    }
}
