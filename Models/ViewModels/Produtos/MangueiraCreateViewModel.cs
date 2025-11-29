using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.ViewModels.Produtos
{
    /// <summary>
    /// ViewModel para criação de Mangueira
    /// </summary>
    public class MangueiraCreateViewModel
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
        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: MGA).")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente é obrigatório.")]
        public string Sku_Agente { get; set; }

        [Required(ErrorMessage = "O Comprimento é obrigatório (Ex: 10MT).")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        public string Sku_Modelo { get; set; }

        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Mangueira
        [Required(ErrorMessage = "O comprimento é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O comprimento deve ser maior que zero.")]
        public decimal Comprimento { get; set; }

        [Required(ErrorMessage = "O diâmetro é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O diâmetro deve ser maior que zero.")]
        public decimal Diametro { get; set; }

        [Required(ErrorMessage = "O tipo de material é obrigatório.")]
        public string TipoMaterial { get; set; }

        public int? PressaoMaxima { get; set; }
    }
}

