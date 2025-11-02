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

        public async Task<List<CarrinhoItem>> ObterItensPorUsuarioAsync(string userId)
        {
            return await _context.CarrinhoItems
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DataAdicionado)
                .ToListAsync();
        }

        public async Task<CarrinhoItem> AdicionarItemAsync(CarrinhoItem item)
        {
            // Verifica se o item já existe no carrinho do usuário
            var itemExistente = await _context.CarrinhoItems
                .FirstOrDefaultAsync(c => c.UserId == item.UserId && c.ProdutoId == item.ProdutoId);

            if (itemExistente != null)
            {
                // Atualiza a quantidade se o item já existe
                itemExistente.Quantidade += item.Quantidade;
                _context.CarrinhoItems.Update(itemExistente);
                await _context.SaveChangesAsync();
                return itemExistente;
            }
            else
            {
                // Adiciona novo item
                await _context.CarrinhoItems.AddAsync(item);
                await _context.SaveChangesAsync();
                return item;
            }
        }

        public async Task<bool> RemoverItensPorUsuarioAsync(string userId)
        {
            var itens = await _context.CarrinhoItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (itens.Any())
            {
                _context.CarrinhoItems.RemoveRange(itens);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<CarrinhoItem> AtualizarQuantidadeAsync(int itemId, int quantidade)
        {
            var item = await _context.CarrinhoItems.FindAsync(itemId);
            if (item != null)
            {
                item.Quantidade = quantidade;
                _context.CarrinhoItems.Update(item);
                await _context.SaveChangesAsync();
                return item;
            }

            return null;
        }

        public async Task<bool> RemoverItemAsync(int itemId)
        {
            var item = await _context.CarrinhoItems.FindAsync(itemId);
            if (item != null)
            {
                _context.CarrinhoItems.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}

