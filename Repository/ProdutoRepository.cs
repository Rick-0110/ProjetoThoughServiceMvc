using Microsoft.EntityFrameworkCore; 
using ToughService.Data;
using ToughService.Models;

namespace ToughService.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly BancoContext _context;

        public ProdutoRepository(BancoContext context)
        {
            _context = context;
        }

    
        public async Task<ProdutoModel> AddProdutoAsync(ProdutoModel produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<IEnumerable<ProdutoModel>> GetAllProdutosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<ProdutoModel> GetProdutoByIdAsync(int id)
        {
            return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> RemoveProdutoAsync(int id)
        {
            var produto = await GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return false;
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProdutoModel>> SearchProdutosAsync(string termobusca)
        {
            if (string.IsNullOrEmpty(termobusca))
            {
                return await GetAllProdutosAsync();
            }

            var termoBuscaLower = termobusca.ToLower();

            return await _context.Produtos
                .Where(p =>
                    p.Nome.ToLower().Contains(termoBuscaLower) ||
                    p.Sku.ToLower().Contains(termoBuscaLower) ||
                    (p.Categoria.HasValue && p.Categoria.ToString().ToLower().Contains(termoBuscaLower))
                )
                .ToListAsync();
        }

        public async Task<ProdutoModel> UpdateProdutoAsync(ProdutoModel produto)
        {
            var produtoDB = await GetProdutoByIdAsync(produto.Id);

            if (produtoDB == null)
            {
                throw new System.Exception("Produto não encontrado para atualização.");
            }
            produtoDB.Nome = produto.Nome;
            produtoDB.Descricao = produto.Descricao;
            produtoDB.Preco = produto.Preco;
            produtoDB.Quantidade = produto.Quantidade;
            produtoDB.Categoria = produto.Categoria;
            produtoDB.Sku = produto.Sku;
            produtoDB.Marca = produto.Marca;
            produtoDB.Peso = produto.Peso;
            produtoDB.Ativo = produto.Ativo;

            if (!string.IsNullOrEmpty(produto.ImagemUrl))
            {
                produtoDB.ImagemUrl = produto.ImagemUrl;
            }

            _context.Produtos.Update(produtoDB);
            await _context.SaveChangesAsync();

            return produtoDB;
        }
    }
}