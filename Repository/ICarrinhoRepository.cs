using ToughService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToughService.Repository
{
    public interface ICarrinhoRepository
    {
        Task<List<CarrinhoItem>> ObterItensPorUsuarioAsync(string userId);
        Task<CarrinhoItem> AdicionarItemAsync(CarrinhoItem item);
        Task<bool> RemoverItensPorUsuarioAsync(string userId);
        Task<CarrinhoItem> AtualizarQuantidadeAsync(int itemId, int quantidade);
        Task<bool> RemoverItemAsync(int itemId);
    }
}


