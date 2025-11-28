using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ToughService.Models.Produtos
{
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
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;

        // --- Campos SKU (Obrigatórios pela Interface) ---
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

        [Required(ErrorMessage = "O peso líquido é obrigatório.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PesoLiquido { get; set; }

        [Required(ErrorMessage = "A data da última recarga é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataRecarga { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataVencimento { get; set; }


        [DataType(DataType.Date)]
        public DateTime? DataFabricacao { get; set; }

        [StringLength(50)]
        public string NormaReferencia { get; set; }

        public string TipoAgente { get; set; }
        public string Capacidade { get; set; }

        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Implementação da interface
        public CategoriaEnum Categoria => CategoriaEnum.Extintores;
    }
}