using ProjetoThoughServiceMvc.Models;
using ToughService.Data;

namespace ToughService.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly BancoContext _context;

        public ProdutoRepository(BancoContext context)
        {
            _context = context;
        }


        public IEnumerable<ProdutoModel> GetAllProdutos()
        {
            return _context.Produtos.ToList();
        }

        public ProdutoModel GetProdutoById(int id)
        {
            return _context.Produtos.FirstOrDefault(p => p.Id == id);
        }

        public void AddProduto(ProdutoModel produto)
        {
            _context.Produtos.Add(produto);
            _context.SaveChanges();
        }

        public void RemoveProduto(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                _context.SaveChanges();
            }
        }
    }
}
