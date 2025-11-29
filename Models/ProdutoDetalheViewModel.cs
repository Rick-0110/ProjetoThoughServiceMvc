using ToughService.Models.Produtos;

namespace ToughService.Models
{
    public class ProdutoDetalheViewModel
    {
        public ProdutoBaseModel Produto { get; set; }
       
        public List<ProdutoBaseModel> OutrosProdutos { get; internal set; }
    }
}