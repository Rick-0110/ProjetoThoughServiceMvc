using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models.Produtos
{
    public class ExtintorModel : ProdutoBaseModel
    {
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PesoLiquido { get; set; }

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

        public ExtintorModel()
        {
            Categoria = CategoriaEnum.Extintores;
        }
    }
}