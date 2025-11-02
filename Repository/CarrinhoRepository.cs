using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                                 .Include(i => i.Produto) 
                                 .Where(i => i.UserId == userId)
                                 .ToListAsync();
        }

        public async Task AddItemAsync(ItemCarrinhoModel item)
        {
            _context.ItensCarrinho.Add(item);
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

        public async Task UpdateItemAsync(ItemCarrinhoModel item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<ItemCarrinhoModel> GetItemAsync(int produtoId, string userId)
        {
            return await _context.ItensCarrinho
                                 .FirstOrDefaultAsync(i => i.ProdutoId == produtoId && i.UserId == userId);
        }

        public async Task ClearCarrinhoAsync(string userId)
        {
            var itens = await _context.ItensCarrinho
                                      .Where(i => i.UserId == userId)
                                      .ToListAsync();

            if (itens.Any())
            {
                _context.ItensCarrinho.RemoveRange(itens);
                await _context.SaveChangesAsync();
            }
        }
    }
}