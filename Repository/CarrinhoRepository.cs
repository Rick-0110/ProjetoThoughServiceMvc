using Microsoft.EntityFrameworkCore;
using ToughService.Data;
using ToughService.Models;

namespace ToughService.Repository
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly BancoContext _context;

        public CarrinhoRepository(BancoContext context)
        {
            _context = context;
        }

        public async Task<List<ItemCarrinhoModel>> GetCarrinhoByUserIdAsync(string userId)
        {
            return await _context.ItensCarrinho
           .Include(c => c.Produto)
           .Where(c => c.UserId == userId)
           .ToListAsync();
        }

        public async Task<ItemCarrinhoModel> GetItemAsync(int produtoId, string userId)
        {
            return await _context.ItensCarrinho
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.ProdutoId == produtoId && c.UserId == userId);

        }

        public async Task AddToCarrinhoAsync(int produtoId, int quantidade, string usuarioId = null)
        {
            var item = await GetItemAsync(produtoId, usuarioId);

            if (item == null)
            {
                // Buscar dados do produto
                var produto = await _context.Produtos.FindAsync(produtoId);
                if (produto == null)
                    throw new Exception("Produto não encontrado.");

                item = new ItemCarrinhoModel
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    UserId = usuarioId
                };

                await AddItemAsync(item);
            }
            else
            {
                item.Quantidade += quantidade;
                await UpdateItemAsync(item);
            }
        }

        public async Task AddItemAsync(ItemCarrinhoModel item)
        {
            _context.ItensCarrinho.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(ItemCarrinhoModel item)
        {
            _context.ItensCarrinho.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int produtoId, string userId)
        {
            var item = await GetItemAsync(produtoId, userId);

            if (item != null)
            {
                _context.ItensCarrinho.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCarrinhoAsync(string userId)
        {
            var itens = _context.ItensCarrinho.Where(c => c.UserId == userId);
            _context.ItensCarrinho.RemoveRange(itens);
            await _context.SaveChangesAsync();
        }
    }
}
