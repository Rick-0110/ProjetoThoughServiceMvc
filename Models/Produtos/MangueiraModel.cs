using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models.Produtos
{
    public class MangueiraModel : ProdutoBaseModel
    {
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Comprimento { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Diametro { get; set; }

        [StringLength(50)]
        public string TipoMaterial { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Peso { get; set; }

        public MangueiraModel()
        {
            Categoria = CategoriaEnum.Mangueiras;
        }
    }
}