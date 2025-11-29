using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models.Produtos
{

    public abstract class ProdutoBaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;

        // --- CATEGORIA (Obrigatória na base) ---
        public int CategoriaId { get; set; }
        public CategoriaEnum Categoria { get; set; }

        // --- SKU (Códigos de Identificação) ---
        [Required(ErrorMessage = "O SKU é obrigatório.")]
        public string Sku { get; set; }

        [Required]
        public string Sku_Tipo { get; set; }
        [Required]
        public string Sku_Agente { get; set; }
        [Required]
        public string Sku_Capacidade { get; set; }
        [Required]
        public string Sku_Modelo { get; set; }

        // --- COMUNS ---
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        [NotMapped]
        public IFormFile? Imagem { get; set; }
    }
}