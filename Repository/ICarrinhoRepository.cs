using ProjetoThoughServiceMvc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToughService.Models;

namespace ToughService.Repository
{
    public interface ICarrinhoRepository
    {

        Task<List<ItemCarrinhoModel>> GetCarrinhoByUserIdAsync(string userId);
        //remover item do carrinho
        Task RemoveItemAsync(int itemId, string userId);

        // Atualiza a quantidade de um item
        Task UpdateQuantidadeAsync(int itemId, int novaQuantidade, string userId);

        // Limpa o carrinho do usuário
        Task ClearCarrinhoAsync(string userId);

    }
}