using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Model específico para Sistemas de Detecção de Fumaça
    /// </summary>
    public class SistemaDeteccaoModel : IProdutoBase
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

        [Required(ErrorMessage = "A Capacidade/Área é obrigatória.")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        public string Sku_Modelo { get; set; }

        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Sistema de Detecção
        [Required(ErrorMessage = "O tipo de detecção é obrigatório.")]
        public string TipoDetecao { get; set; } // Fumaça, Calor, Gás, etc.

        public string? AreaCobertura { get; set; } // Área de cobertura em m²
        public string? TipoSensor { get; set; } // Óptico, Ionização, Térmico, etc.
        public string? TensaoAlimentacao { get; set; } // 12V, 24V, 220V, etc.
        public string? Norma { get; set; } // ABNT, NFPA, etc.

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.SistemasDeDeteccao;
    }
}

