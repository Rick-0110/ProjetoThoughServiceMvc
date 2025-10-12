using ProjetoThoughServiceMvc.Models;
using System.Linq;
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

        public IEnumerable<ProdutoModel> SearchProdutos(string termobusca)
        {
            if (string.IsNullOrEmpty(termobusca))
            {
                return GetAllProdutos();
            }
            var categoriasCorrespondentes = Enum.GetValues(typeof(CategoriaEnum))
           .Cast<CategoriaEnum>()
           .Where(cat => cat.ToString().Contains(termobusca, StringComparison.OrdinalIgnoreCase))
           .ToList();

          
            return _context.Produtos

                .Where(p =>
                    p.Nome.Contains(termobusca) ||
                  
                    categoriasCorrespondentes.Contains(p.Categoria.Value)
                )
                .ToList();
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
