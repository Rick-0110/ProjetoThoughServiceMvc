using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToughService.Data;
using ToughService.Models.Produtos; // Importante

namespace ToughService.Repository
{
    public class ProdutoRepositoryGeneric : IProdutoRepositoryGeneric
    {
        private readonly BancoContext _context;

        public ProdutoRepositoryGeneric(BancoContext context)
        {
            _context = context;
        }

        // Mude 'where T : IProdutoBase' para 'where T : ProdutoBaseModel'
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : ProdutoBaseModel
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : ProdutoBaseModel
        {
            return await _context.Set<T>().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<T> AddAsync<T>(T produto) where T : ProdutoBaseModel
        {
            await _context.Set<T>().AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<bool> RemoveAsync<T>(int id) where T : ProdutoBaseModel
        {
            var produto = await GetByIdAsync<T>(id);
            if (produto == null) return false;

            _context.Set<T>().Remove(produto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T> UpdateAsync<T>(T produto) where T : ProdutoBaseModel
        {
            _context.Set<T>().Update(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        // Método Global
        public async Task<IEnumerable<ProdutoBaseModel>> GetAllProdutosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<string> VerificarSkuExistenteAsync(string sku)
        {
            var produto = await _context.Produtos
                                        .Select(p => new { p.Sku })
                                        .FirstOrDefaultAsync(p => p.Sku == sku);
            return produto?.Sku;
        }
    }
}