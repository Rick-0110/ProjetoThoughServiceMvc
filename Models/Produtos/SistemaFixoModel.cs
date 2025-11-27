using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Model específico para Sistemas Fixos de Combate a Incêndio
    /// </summary>
    public class SistemaFixoModel : IProdutoBase
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

        // Campos específicos de Sistema Fixo
        [Required(ErrorMessage = "O tipo de sistema é obrigatório.")]
        public string TipoSistema { get; set; } // Sprinkler, Hidrante, Espuma, etc.

        public string? CapacidadeSistema { get; set; } // Capacidade total do sistema
        public string? AreaCobertura { get; set; } // Área de cobertura em m²
        public string? PressaoTrabalho { get; set; } // Pressão de trabalho
        public string? Norma { get; set; } // ABNT, NFPA, etc.

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.SistemasFixos;
    }
}

