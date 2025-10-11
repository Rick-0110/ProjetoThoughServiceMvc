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
    public class ADMController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public ADMController(IProdutoRepository produtoRepository, UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
        }

        public IActionResult ADM()
        {
            return View();
        }

        public IActionResult AdmChamados()
        {
            return View();
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
    }
}
