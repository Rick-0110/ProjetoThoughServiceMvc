using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToughService.Models.Produtos;

namespace ToughService.Models
{
    public class ProdutoModel : IProdutoBase
    {
        public int Id { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        public int CategoriaId { get; set; }
        public CategoriaEnum Categoria { get; set; }

        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: EAT).")]
        public string Sku_Tipo { get; set; } // Ex: EAT (Extintor Automático) / MGA (Mangueira)

        [Required(ErrorMessage = "O Agente Extintor é obrigatório (Ex: PO / CO2).")]
        public string Sku_Agente { get; set; } // Ex: PO (Pó Químico) / AGUA

        [Required(ErrorMessage = "A Capacidade é obrigatória (Ex: 06KG / 10MT).")]
        public string Sku_Capacidade { get; set; } // Ex: 06KG (Capacidade) / 10MT (Comprimento da Mangueira)

        [Required(ErrorMessage = "O Modelo é obrigatório (Ex: MOD2).")]
        public string Sku_Modelo { get; set; } // Identificador de modelo/fabricante interno
        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; } 
        public decimal? Peso { get; set; } 
        public bool Ativo { get; set; }

        [NotMapped] 
        public IFormFile? Imagem { get; set; }

    }
}