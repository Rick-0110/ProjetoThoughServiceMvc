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
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IProdutoRepository produtoRepository, UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var todosProdutos = await _produtoRepository.GetAllProdutosAsync();
            
            // Filtra apenas produtos ativos
            var produtosAtivos = todosProdutos.Where(p => p.Ativo).ToList();
            
            // Se não houver produtos ativos, mostra todos (para compatibilidade com produtos antigos)
            if (!produtosAtivos.Any())
            {
                produtosAtivos = todosProdutos.ToList();
            }
            
            // Agrupa produtos por categoria (produtos sem categoria vão para Extintores por padrão)
            var produtosPorCategoria = produtosAtivos
                .GroupBy(p => p.Categoria ?? ToughService.Models.CategoriaEnum.Extintores)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            // Cria lista de todas as categorias disponíveis (mesmo sem produtos)
            var todasCategorias = Enum.GetValues(typeof(ToughService.Models.CategoriaEnum))
                .Cast<ToughService.Models.CategoriaEnum>()
                .ToList();
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _userManager.FindByIdAsync(userId);

            ViewBag.IsLoggedIn = usuario != null;
            ViewBag.ProdutosPorCategoria = produtosPorCategoria;
            ViewBag.TodasCategorias = todasCategorias; // Passa todas as categorias para a view

            return View(produtosAtivos);
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
