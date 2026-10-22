using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISII.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategoriaAProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Products",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoriaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoriaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoriaId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoriaId",
                table: "Products",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categorias_CategoriaId",
                table: "Products",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categorias_CategoriaId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoriaId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Products");
        }
    }
}
