using ToughService.Models.Produtos; 

namespace ToughService.Repository
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<ProdutoBaseModel>> GetAllProdutosAsync();

        Task<ProdutoBaseModel> GetProdutoByIdAsync(int id);

        Task<ProdutoBaseModel> AddProdutoAsync(ProdutoBaseModel produto);
        Task<ProdutoBaseModel> UpdateProdutoAsync(ProdutoBaseModel produto);
        Task<bool> RemoveProdutoAsync(int id);

        // Busca
        Task<IEnumerable<ProdutoBaseModel>> SearchProdutosAsync(string termo);
    }
}