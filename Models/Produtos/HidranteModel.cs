using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Model específico para Hidrantes
    /// </summary>
    public class HidranteModel : IProdutoBase
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
        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório.")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente é obrigatório.")]
        public string Sku_Agente { get; set; }

        [Required(ErrorMessage = "A Capacidade é obrigatória.")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        public string Sku_Modelo { get; set; }

        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Hidrante
        [Required(ErrorMessage = "O tipo de hidrante é obrigatório.")]
        public string TipoHidrante { get; set; } // Coluna, Parede, Subterrâneo

        [Required(ErrorMessage = "A pressão de trabalho é obrigatória.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A pressão deve ser maior que zero.")]
        public decimal PressaoTrabalho { get; set; } // Em Bar ou PSI

        public string Rosca { get; set; } // Tipo de rosca (NPT, BSP, etc.)
        public string Material { get; set; } // Ferro fundido, Aço, etc.

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.Hidrantes;
    }
}

