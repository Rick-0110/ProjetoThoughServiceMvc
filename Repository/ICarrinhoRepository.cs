using ToughService.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToughService.Models;

namespace ToughService.Repository
{
    public interface ICarrinhoRepository
    {

        Task<List<ItemCarrinhoModel>> GetCarrinhoByUserIdAsync(string userId);
        Task AddItemAsync(ItemCarrinhoModel item);
        Task RemoveItemAsync(int itemId, string userId);

        // Atualiza a quantidade de um item
        Task UpdateItemAsync(ItemCarrinhoModel item);

        // Busca um item específico (ex: para ver se já existe)
        Task<ItemCarrinhoModel> GetItemAsync(int produtoId, string userId);

        // Limpa o carrinho do usuário
        Task ClearCarrinhoAsync(string userId);



    }
}