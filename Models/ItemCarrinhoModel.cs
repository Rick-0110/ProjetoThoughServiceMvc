using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoThoughServiceMvc.Models
{
    public class ItemCarrinhoModel
    {
        public int ProdutoId { get; set; }
        public int Id { get; set; }
        public string NomeProduto { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }

        public string ImagemUrl { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual ProdutoModel Produto { get; set; }
    }
}
