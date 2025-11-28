using System.Collections.Generic;
using System.Threading.Tasks;
using ToughService.Models.Produtos;

namespace ToughService.Repository
{
    public interface IProdutoRepositoryGeneric
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : ProdutoBaseModel;
        Task<T> GetByIdAsync<T>(int id) where T : ProdutoBaseModel;
        Task<T> AddAsync<T>(T produto) where T : ProdutoBaseModel;
        Task<bool> RemoveAsync<T>(int id) where T : ProdutoBaseModel;
        Task<T> UpdateAsync<T>(T produto) where T : ProdutoBaseModel;

        Task<IEnumerable<ProdutoBaseModel>> GetAllProdutosAsync();
        Task<string> VerificarSkuExistenteAsync(string sku);
    }
}