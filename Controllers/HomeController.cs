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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoverProduto(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Registro");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null || !usuario.EhAdmin)
                return RedirectToAction("Login", "Registro");

            _produtoRepository.RemoveProduto(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AdicionarProdutoADM()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _userManager.FindByIdAsync(userId);

            if (usuario == null || !usuario.EhAdmin)
                return RedirectToAction("Login", "Registro");

            ViewBag.UsuarioEhAdmin = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarProdutoADM(ProdutoModel produto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Registro");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null || !usuario.EhAdmin)
                return RedirectToAction("Login", "Registro");

            if (ModelState.IsValid)
            {
                _produtoRepository.AddProduto(produto);
                return RedirectToAction("Index");
            }

            return View(produto);
        }

        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
