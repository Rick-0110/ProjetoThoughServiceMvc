using ToughService.Models;

namespace ToughService.Repository
{
    public interface IProdutoRepository
    {
     Task<IEnumerable<ProdutoModel>> GetAllProdutosAsync();
        Task<ProdutoModel> GetProdutoByIdAsync(int id);
        Task<ProdutoModel> AddProdutoAsync(ProdutoModel produto);
        Task<bool> RemoveProdutoAsync(int id);
        Task<ProdutoModel> UpdateProdutoAsync(ProdutoModel produto);
        Task<IEnumerable<ProdutoModel>> SearchProdutosAsync(string termobusca);
    }
}
