using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToughService.Models.Produtos;

namespace ToughService.Models
{
    [Table("ItensCarrinho")]
    public class ItemCarrinhoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual ProdutoBaseModel Produto { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }


        [NotMapped]
        public decimal Total
        {
            get { return Quantidade * PrecoUnitario; }
            set {  }
        }
    }
}