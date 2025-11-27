using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToughService.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposSkuEmProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sku_Agente",
                table: "Produtos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Sku_Capacidade",
                table: "Produtos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Sku_Modelo",
                table: "Produtos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Sku_Tipo",
                table: "Produtos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sku_Agente",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Sku_Capacidade",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Sku_Modelo",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Sku_Tipo",
                table: "Produtos");
        }
    }
}
