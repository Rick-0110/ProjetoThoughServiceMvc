using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Model específico para Extintores de Incêndio
    /// </summary>
    public class ExtintorModel : IProdutoBase
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
        [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: EAT).")]
        public string Sku_Tipo { get; set; }

        [Required(ErrorMessage = "O Agente Extintor é obrigatório (Ex: PO / CO2).")]
        public string Sku_Agente { get; set; }

        [Required(ErrorMessage = "A Capacidade é obrigatória (Ex: 06KG).")]
        public string Sku_Capacidade { get; set; }

        [Required(ErrorMessage = "O Modelo é obrigatório (Ex: MOD2).")]
        public string Sku_Modelo { get; set; }

        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }

        // Campos específicos de Extintor
        [Required(ErrorMessage = "O peso é obrigatório para extintores.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O peso deve ser maior que zero.")]
        public decimal Peso { get; set; } // Em kg

        [Required(ErrorMessage = "A data da última recarga é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataRecarga { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataVencimento { get; set; }

        public string TipoAgente { get; set; } // PO, CO2, AGUA, ESPUMA
        public string Capacidade { get; set; } // 06KG, 10KG, etc.

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.Extintores;
    }
}

