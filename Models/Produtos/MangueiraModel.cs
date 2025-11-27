using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Model específico para Mangueiras de Combate a Incêndio
    /// </summary>
    public class MangueiraModel : IProdutoBase
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;

        // Campos SKU
        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: MGA).")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente é obrigatório.")]
        public string Sku_Agente { get; set; }

        [Required(ErrorMessage = "O Comprimento é obrigatório (Ex: 10MT).")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        public string Sku_Modelo { get; set; }

        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Mangueira
        [Required(ErrorMessage = "O comprimento é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O comprimento deve ser maior que zero.")]
        public decimal Comprimento { get; set; } // Em metros

        [Required(ErrorMessage = "O diâmetro é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O diâmetro deve ser maior que zero.")]
        public decimal Diametro { get; set; } // Em polegadas ou mm

        public string TipoMaterial { get; set; } // Lona, Sintética, etc.
        public int? PressaoMaxima { get; set; } // Em PSI ou Bar

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.Mangueiras;
    }
}

