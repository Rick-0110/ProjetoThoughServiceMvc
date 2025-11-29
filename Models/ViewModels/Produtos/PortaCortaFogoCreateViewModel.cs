using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.ViewModels.Produtos
{
    /// <summary>
    /// ViewModel para criação de Porta Corta-Fogo
    /// </summary>
    public class PortaCortaFogoCreateViewModel
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

        [Required(ErrorMessage = "A Capacidade/Dimensão é obrigatória.")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        public string Sku_Modelo { get; set; }

        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Porta Corta-Fogo
        [Required(ErrorMessage = "A largura é obrigatória.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A largura deve ser maior que zero.")]
        public decimal Largura { get; set; }

        [Required(ErrorMessage = "A altura é obrigatória.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A altura deve ser maior que zero.")]
        public decimal Altura { get; set; }

        [Required(ErrorMessage = "O material é obrigatório.")]
        public string Material { get; set; }

        [Required(ErrorMessage = "O tempo de resistência ao fogo é obrigatório.")]
        public string TempoResistenciaFogo { get; set; }

        public string? TipoAbertura { get; set; }
        public string? Certificacao { get; set; }
    }
}

