using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models
{
    [Table("PedidoItens")]
    public class PedidoItemModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [ForeignKey("PedidoId")]
        public virtual PedidoModel Pedido { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual ProdutoModel Produto { get; set; }
    }
}

