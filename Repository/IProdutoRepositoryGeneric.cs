using System.Collections.Generic;
using System.Threading.Tasks;
using ToughService.Models.Produtos;

namespace ToughService.Repository
{
    /// <summary>
    /// Interface genérica para repositório de produtos que trabalha com IProdutoBase
    /// </summary>
    public interface IProdutoRepositoryGeneric
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class, IProdutoBase;
        Task<T> GetByIdAsync<T>(int id) where T : class, IProdutoBase;
        Task<T> AddAsync<T>(T produto) where T : class, IProdutoBase;
        Task<bool> RemoveAsync<T>(int id) where T : class, IProdutoBase;
        Task<T> UpdateAsync<T>(T produto) where T : class, IProdutoBase;
        Task<IEnumerable<IProdutoBase>> GetAllProdutosAsync();
        Task<string> VerificarSkuExistenteAsync(string sku);
    }
}

