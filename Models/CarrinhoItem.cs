using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models
{
    public class CarrinhoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public string NomeProduto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        [Required]
        public int Quantidade { get; set; }

        public string ImagemUrl { get; set; }

        public DateTime DataAdicionado { get; set; } = DateTime.Now;
    }
}


