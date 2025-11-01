using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToughService.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoDefinitivaDoCarrinho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Preco",
                table: "ItensCarrinho");

            migrationBuilder.DropColumn(
                name: "NomeProduto",
                table: "ItensCarrinho");

            migrationBuilder.DropColumn(
                name: "ImagemUrl", 
                table: "ItensCarrinho");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
