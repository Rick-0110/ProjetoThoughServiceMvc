using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models.Produtos
{
    public class HidranteModel : ProdutoBaseModel
    {
        [Required]
        [StringLength(100)]
        public string TipoHidrante { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PressaoTrabalho { get; set; }

        [StringLength(50)]
        public string Rosca { get; set; }

        [StringLength(50)]
        public string Material { get; set; }

        public HidranteModel()
        {
            Categoria = CategoriaEnum.Hidrantes;
        }
    }
}