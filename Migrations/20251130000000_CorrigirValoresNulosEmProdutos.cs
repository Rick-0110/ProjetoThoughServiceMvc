using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToughService.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirValoresNulosEmProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Atualiza registros com Categoria NULL para o valor padrão 0 (Extintores)
            migrationBuilder.Sql(@"
                UPDATE Produtos 
                SET Categoria = 0, CategoriaId = 0 
                WHERE Categoria IS NULL OR CategoriaId IS NULL
            ");

            // Atualiza registros com Quantidade NULL para 0
            migrationBuilder.Sql(@"
                UPDATE Produtos 
                SET Quantidade = 0 
                WHERE Quantidade IS NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Não há necessidade de reverter esta migração de dados
        }
    }
}

