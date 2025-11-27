using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Interface base para todos os produtos, contendo campos comuns
    /// </summary>
    public interface IProdutoBase
    {
        int Id { get; set; }
        string Nome { get; set; }
        string Descricao { get; set; }
        decimal Preco { get; set; }
        string ImagemUrl { get; set; }
        string Sku { get; set; }
        string Marca { get; set; }
        int Quantidade { get; set; }
        bool Ativo { get; set; }
        
        // Campos para geração de SKU
        string Sku_Tipo { get; set; }
        string Sku_Agente { get; set; }
        string Sku_Capacidade { get; set; }
        string Sku_Modelo { get; set; }
        
        // Categoria
        CategoriaEnum Categoria { get; }
    }
}

