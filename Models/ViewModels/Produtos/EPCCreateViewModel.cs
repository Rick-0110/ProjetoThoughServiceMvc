using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.ViewModels.Produtos
{
    /// <summary>
    /// ViewModel para criação de EPC
    /// </summary>
    public class EPCCreateViewModel
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        public IFormFile? Imagem { get; set; }

        // Campos SKU
        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório.")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente é obrigatório.")]
        public string Sku_Agente { get; set; }

        [Required(ErrorMessage = "A Capacidade/Área é obrigatória.")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        public string Sku_Modelo { get; set; }

        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de EPC
        [Required(ErrorMessage = "O tipo de EPC é obrigatório.")]
        public string TipoEPC { get; set; }

        public string? AreaProtecao { get; set; }
        public string? Dimensoes { get; set; }
        public string? Material { get; set; }
        public string? Norma { get; set; }
    }
}

