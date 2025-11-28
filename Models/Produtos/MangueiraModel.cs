using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Model específico para Mangueiras de Incêndio
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


        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: MANG).")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente/Material é obrigatório (Ex: SINTETICO).")]
        public string Sku_Agente { get; set; } // Para mangueiras, pode ser usado para o Material

        [Required(ErrorMessage = "A Capacidade/Comprimento é obrigatória (Ex: 15MT).")]
        public string Sku_Capacidade { get; set; } // Usado para o comprimento no SKU

        [Required(ErrorMessage = "O Modelo/Diâmetro é obrigatório (Ex: 1.5POL).")]
        public string Sku_Modelo { get; set; } // Usado para o diâmetro no SKU

        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // --- Campos Específicos de Mangueiras ---

        [Required(ErrorMessage = "O comprimento é obrigatório.")]
        [Range(1, 100, ErrorMessage = "O comprimento deve estar entre 1 e 100 metros.")]
        public decimal Comprimento { get; set; } // Ex: 15, 20, 30 (metros)

        [Required(ErrorMessage = "O diâmetro é obrigatório.")]
        public decimal Diametro { get; set; } // Ex: 1.5, 2.5 (polegadas)

        [Required(ErrorMessage = "O tipo do material é obrigatório.")]
        [StringLength(50)]
        public string TipoMaterial { get; set; } // Ex: Tipo 1 (Predial), Tipo 2 (Industrial), Borracha

        public decimal? Peso { get; set; } // Peso do rolo da mangueira

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.Mangueiras;
    }
}