using ToughService.Models;

namespace ToughService.Repository
{
    public interface IPedidoRepository
    {
        Task AddPedidoAsync(PedidoModel pedido);
        Task<IEnumerable<PedidoModel>> GetAllPedidosAsync();
        Task<PedidoModel> GetPedidoByIdAsync(int pedidoId);
        Task UpdateStatusPedidoAsync(int pedidoId, StatusPedidoEnum novoStatus);
        Task UpdatePedidoAsync(PedidoModel pedido);
        Task<List<PedidoModel>> GetPedidosByUserIdAsync(string userId);
    }
}

