using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using ToughService.Models;
using ToughService.Repository;

namespace ToughService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        // REMOVIDO: IProdutoRepositoryGeneric (não é mais necessário para listagem)
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. BUSCA UNIFICADA (Tabela Única resolve tudo!)
            // Isso traz Extintores, Mangueiras e Genéricos, tudo junto.
            var todosProdutos = await _produtoRepository.GetAllProdutosAsync();

            // 2. FILTRO DE ATIVOS
            var produtosAtivos = todosProdutos.Where(p => p.Ativo).ToList();

            // Fallback: se não tiver ativos, mostra tudo (para teste)
            if (!produtosAtivos.Any())
            {
                produtosAtivos = todosProdutos.ToList();
            }

            // 3. AGRUPAMENTO (Funciona direto com ProdutoBaseModel)
            var produtosPorCategoria = produtosAtivos
                .GroupBy(p => p.Categoria)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 4. DADOS AUXILIARES
            var todasCategorias = Enum.GetValues(typeof(CategoriaEnum))
                .Cast<CategoriaEnum>()
                .ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = userId != null ? await _userManager.FindByIdAsync(userId) : null;

            ViewBag.IsLoggedIn = usuario != null;
            ViewBag.ProdutosPorCategoria = produtosPorCategoria;
            ViewBag.TodasCategorias = todasCategorias;

            // RETORNA LISTA DE BASE (ProdutoBaseModel)
            return View(produtosAtivos);
        }

        // ... (Sobre e Error continuam iguais) ...

        [HttpGet]
        public async Task<IActionResult> Buscar(string busca)
        {
            ViewBag.TermoBuscado = busca;

            if (string.IsNullOrWhiteSpace(busca))
            {
                return RedirectToAction("Index");
            }

            // O repositório já devolve IEnumerable<ProdutoBaseModel>
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