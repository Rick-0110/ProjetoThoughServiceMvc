using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using System.Diagnostics;
using System.Security.Claims;
using ToughService.Data;
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
            var produtos = _produtoRepository.GetAllProdutos();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _userManager.FindByIdAsync(userId);

            ViewBag.UsuarioEhAdmin = usuario != null && usuario.EhAdmin;
            ViewBag.IsLoggedIn = usuario != null;


            return View(produtos);
        }

       

        public IActionResult Sobre()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Buscar(string busca)
        {
         
            var produtos = string.IsNullOrEmpty(busca)
                ? _produtoRepository.GetAllProdutos()
                : _produtoRepository.GetAllProdutos()
                    .Where(p => p.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase)
                             || p.Categoria.Contains(busca, StringComparison.OrdinalIgnoreCase))
                    .ToList();

           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _userManager.FindByIdAsync(userId);

            ViewBag.UsuarioEhAdmin = usuario != null && usuario.EhAdmin;
            ViewBag.IsLoggedIn = usuario != null;

            return View("Index", produtos); 
        }

        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
