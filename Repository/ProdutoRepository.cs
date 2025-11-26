using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            try
            {
                Console.WriteLine($"REPOSITÓRIO: Adicionando produto ao contexto. Nome: {produto.Nome}");
                await _context.Produtos.AddAsync(produto);
                Console.WriteLine($"REPOSITÓRIO: Produto adicionado ao contexto. Salvando mudanças...");
                
                int linhasAfetadas = await _context.SaveChangesAsync();
                Console.WriteLine($"REPOSITÓRIO: SaveChangesAsync retornou {linhasAfetadas} linha(s) afetada(s)");
                Console.WriteLine($"REPOSITÓRIO: Produto salvo com ID: {produto.Id}");
                
                if (linhasAfetadas > 0)
                {
                    return produto;
                }
                else
                {
                    throw new Exception("Nenhuma linha foi afetada ao salvar o produto.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO NO REPOSITÓRIO AO ADICIONAR PRODUTO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
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

        public async Task<IEnumerable<ProdutoModel>> SearchProdutosAsync(string termoBusca)
        {
            var termoBuscaLower = termoBusca.ToLower();

            var todosProdutos = await _context.Produtos.ToListAsync();

            var resultados = todosProdutos
                    .Where(p => (p.Nome != null && p.Nome.ToLower().Contains(termoBuscaLower)) ||
                                (p.Sku != null && p.Sku.ToLower().Contains(termoBuscaLower)) ||
                                (p.Categoria.HasValue && p.Categoria.ToString().ToLower().Contains(termoBuscaLower)));

            return resultados.ToList();
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