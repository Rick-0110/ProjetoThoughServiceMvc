

using ProjetoThoughServiceMvc.Models;

namespace ToughService.Repository
{
    public interface IProdutoRepository
    {
        IEnumerable<ProdutoModel> GetAllProdutos();
        IEnumerable<ProdutoModel> SearchProdutos(string termobusca);
        ProdutoModel GetProdutoById(int id);
        void AddProduto(ProdutoModel produto);
        void RemoveProduto(int id);
    }
}
