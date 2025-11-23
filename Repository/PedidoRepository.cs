using Microsoft.EntityFrameworkCore;
using ToughService.Data;
using ToughService.Models;

namespace ToughService.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly BancoContext _context;

        public PedidoRepository(BancoContext context)
        {
            _context = context;
        }

        public async Task AddPedidoAsync(PedidoModel pedido)
        {
            try
            {
                await _context.Pedidos.AddAsync(pedido);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar pedido no repositório: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<PedidoModel>> GetAllPedidosAsync()
        {
            return await _context.Pedidos
                                 .Include(p => p.User)
                                 .Include(p => p.Itens)
                                     .ThenInclude(i => i.Produto)
                                 .ToListAsync();
        }

        public async Task<PedidoModel> GetPedidoByIdAsync(int pedidoId)
        {
            return await _context.Pedidos
                                 .Include(p => p.User)
                                 .Include(p => p.Itens)
                                     .ThenInclude(i => i.Produto)
                                 .FirstOrDefaultAsync(p => p.Id == pedidoId);
        }

        public async Task UpdateStatusPedidoAsync(int pedidoId, StatusPedidoEnum novoStatus)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"Pedido com ID {pedidoId} não foi encontrado.");
            }

            pedido.Status = novoStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<List<PedidoModel>> GetPedidosByUserIdAsync(string userId)
        {
            return await _context.Pedidos
                                 .Include(p => p.Itens)
                                     .ThenInclude(i => i.Produto)
                                 .Where(p => p.UserId == userId)
                                 .OrderByDescending(p => p.DataPedido)
                                 .ToListAsync();
        }
    }
}

