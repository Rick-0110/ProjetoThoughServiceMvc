using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ProjetoThoughServiceMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using ToughService.Data;

namespace ToughService.Controllers
{
    public class HomeController : Controller
    {
        private readonly BancoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(BancoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        [HttpPost]
        public async Task<IActionResult> RemoverProduto(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Registro");

            var usuario = await _userManager.FindByIdAsync(userId.Value.ToString());

            if (usuario == null || !usuario.EhAdmin)
                return RedirectToAction("Login", "Registro");

            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);

            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                _context.SaveChanges();
            }

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
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Registro");

            var usuario = await _userManager.FindByIdAsync(userId.Value.ToString());

            if (usuario == null || !usuario.EhAdmin)
                return RedirectToAction("Login", "Registro");

            if (ModelState.IsValid)
            {
                _context.Produtos.Add(produto);
                _context.SaveChanges();
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
