using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.ViewModels.Produtos
{
    /// <summary>
    /// ViewModel para criação de Extintor
    /// </summary>
    public class ExtintorCreateViewModel
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
        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: EAT).")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente Extintor é obrigatório (Ex: PO / CO2).")]
        public string Sku_Agente { get; set; }

        [Required(ErrorMessage = "A Capacidade é obrigatória (Ex: 06KG).")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório (Ex: MOD2).")]
        public string Sku_Modelo { get; set; }

        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Extintor
        [Required(ErrorMessage = "O peso é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O peso deve ser maior que zero.")]
        public decimal Peso { get; set; }

        [Required(ErrorMessage = "A data da última recarga é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataRecarga { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataVencimento { get; set; }

        [Required(ErrorMessage = "O tipo de agente é obrigatório.")]
        public string TipoAgente { get; set; }

        [Required(ErrorMessage = "A capacidade é obrigatória.")]
        public string Capacidade { get; set; }
    }
}

