
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToughService.Data;
using ToughService.Models;

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
        public int ProdutoId { get; set; } 

        [Required]
        public string UserId { get; set; } 


        [ForeignKey("ProdutoId")]
        public virtual ProdutoModel Produto { get; set; } 

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}