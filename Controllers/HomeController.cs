using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using ToughService.Models;
using ToughService.Repository;

namespace ToughService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoRepositoryGeneric _produtoRepositoryGeneric;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            IProdutoRepository produtoRepository, 
            IProdutoRepositoryGeneric produtoRepositoryGeneric,
            UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _produtoRepositoryGeneric = produtoRepositoryGeneric;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Busca produtos da tabela antiga (Produtos)
            var produtosAntigos = await _produtoRepository.GetAllProdutosAsync();
            
            // Busca produtos das tabelas específicas por categoria
            var produtosEspecificos = await _produtoRepositoryGeneric.GetAllProdutosAsync();
            
            // Converte IProdutoBase para ProdutoModel para exibição na view
            var produtosConvertidos = produtosEspecificos.Select(p => ConverterParaProdutoModel(p)).ToList();
            
            // Combina todos os produtos
            var todosProdutos = produtosAntigos.Concat(produtosConvertidos).ToList();
            
            var produtosAtivos = todosProdutos.Where(p => p != null && p.Ativo).ToList();
            
            if (!produtosAtivos.Any())
            {
                produtosAtivos = todosProdutos.Where(p => p != null).ToList();
            }
            
            var produtosPorCategoria = produtosAtivos
                .GroupBy(p => p.Categoria)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            var todasCategorias = Enum.GetValues(typeof(ToughService.Models.CategoriaEnum))
                .Cast<ToughService.Models.CategoriaEnum>()
                .ToList();
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _userManager.FindByIdAsync(userId);

            ViewBag.IsLoggedIn = usuario != null;
            ViewBag.ProdutosPorCategoria = produtosPorCategoria;
            ViewBag.TodasCategorias = todasCategorias; 

            return View(produtosAtivos);
        }
        
        // Método auxiliar para converter IProdutoBase para ProdutoModel
        private ProdutoModel ConverterParaProdutoModel(ToughService.Models.Produtos.IProdutoBase produtoBase)
        {
            if (produtoBase == null) return null;
            
            return new ProdutoModel
            {
                Id = produtoBase.Id,
                Nome = produtoBase.Nome,
                Descricao = produtoBase.Descricao,
                Preco = produtoBase.Preco,
                ImagemUrl = produtoBase.ImagemUrl,
                Sku = produtoBase.Sku,
                Marca = produtoBase.Marca,
                Quantidade = produtoBase.Quantidade,
                Ativo = produtoBase.Ativo,
                Sku_Tipo = produtoBase.Sku_Tipo,
                Sku_Agente = produtoBase.Sku_Agente,
                Sku_Capacidade = produtoBase.Sku_Capacidade,
                Sku_Modelo = produtoBase.Sku_Modelo,
                Categoria = produtoBase.Categoria,
                CategoriaId = (int)produtoBase.Categoria
            };
        }

       

        public IActionResult Sobre()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Buscar(string busca)
        {
         
            ViewBag.TermoBuscado = busca;

            if (busca == null)
            {
                return RedirectToAction("Index");
            }

            var produtos = await _produtoRepository.SearchProdutosAsync(busca);


            return View("ResultadoBusca", produtos);

           
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
