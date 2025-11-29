using System.ComponentModel.DataAnnotations.Schema;
using ToughService.Models.Produtos; 

namespace ToughService.Models
{
    public class ProdutoModel : ProdutoBaseModel
    {
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Peso { get; set; }

        public ProdutoModel()
        {
            
        }
    }
}