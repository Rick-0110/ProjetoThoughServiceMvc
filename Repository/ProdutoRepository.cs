using Microsoft.EntityFrameworkCore;
using ToughService.Data;
using ToughService.Models.Produtos; 

namespace ToughService.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly BancoContext _context;

        public ProdutoRepository(BancoContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ProdutoBaseModel>> GetAllProdutosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<ProdutoBaseModel> GetProdutoByIdAsync(int id)
        {
            return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProdutoBaseModel> AddProdutoAsync(ProdutoBaseModel produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<bool> RemoveProdutoAsync(int id)
        {
            var produto = await GetProdutoByIdAsync(id);
            if (produto == null) return false;

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProdutoBaseModel>> SearchProdutosAsync(string termoBusca)
        {
            if (string.IsNullOrEmpty(termoBusca))
                return await GetAllProdutosAsync();

            var termoLower = termoBusca.ToLower();

            return await _context.Produtos
                .Where(p => p.Nome.ToLower().Contains(termoLower) ||
                            p.Sku.ToLower().Contains(termoLower))
                .ToListAsync();
        }

        public async Task<ProdutoBaseModel> UpdateProdutoAsync(ProdutoBaseModel produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
            return produto;
        }
    }
}