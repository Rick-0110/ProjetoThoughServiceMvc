using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Extensions;


namespace ToughService.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CarrinhoController(
            ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ItemCarrinhoModel> carrinho;
            var session = _httpContextAccessor.HttpContext.Session;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                carrinho = await _carrinhoRepository.GetCarrinhoByUserIdAsync(user.Id);
            }
            else
            {
                carrinho = session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();

                if (carrinho.Any())
                {
                    foreach (var item in carrinho)
                    {
                        if (item.Produto == null)
                        {
                            item.Produto = await _produtoRepository.GetProdutoByIdAsync(item.ProdutoId);
                        }

    
                    }
                }
            }

            decimal total = 0;
            if (carrinho != null && carrinho.Any())
            {
                total = carrinho.Sum(item => (item.Produto?.Preco ?? 0) * item.Quantidade);
            }
            ViewBag.TotalCarrinho = total;

            return View(carrinho);
        }

        [Route("Carrinho/InfoProduto/{id}")] 
        [HttpGet]
        public async Task<IActionResult> InfoProduto(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var produtoPrincipal = await _produtoRepository.GetProdutoByIdAsync(id);

            if (produtoPrincipal == null)
            {
                TempData["Erro"] = "Produto não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            var todosOsProdutos = await _produtoRepository.GetAllProdutosAsync();
            var produtosRelacionados = todosOsProdutos
                .Where(p => p.Categoria == produtoPrincipal.Categoria && p.Id != id)
                .Take(4)
                .ToList();

            var viewModel = new ToughService.Models.ProdutoDetalheViewModel
            {
                Produto = produtoPrincipal,
                OutrosProdutos = produtosRelacionados
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarAoCarrinho(int produtoId, int quantidade = 1)
        {
            try
            {
                if (_produtoRepository == null)
                {
                    throw new Exception("_produtoRepository está NULO. Verifique o 'AddScoped' no Program.cs.");
                }

                var produto = await _produtoRepository.GetProdutoByIdAsync(produtoId);
                if (produto == null)
                {
                    TempData["ErroCarrinho"] = "Produto não encontrado.";
                    return Redirect(Request.Headers["Referer"].ToString() ?? "/");
                }

                if (User.Identity.IsAuthenticated)
                {
                    if (_userManager == null)
                    {
                        throw new Exception("_userManager está NULO. Verifique o 'AddDefaultIdentity' no Program.cs.");
                    }
                    var user = await _userManager.GetUserAsync(User);

                    if (_carrinhoRepository == null)
                    {
                        throw new Exception("_carrinhoRepository está NULO. Verifique o 'AddScoped' no Program.cs.");
                    }
                    var itemExistente = await _carrinhoRepository.GetItemAsync(produtoId, user.Id);

                    if (itemExistente != null)
                    {
                        itemExistente.Quantidade += quantidade;
                        await _carrinhoRepository.UpdateItemAsync(itemExistente);
                    }
                    else
                    {
                        var novoItem = new ItemCarrinhoModel
                        {
                            ProdutoId = produtoId,
                            Quantidade = quantidade,
                            UserId = user.Id
                        };
                        await _carrinhoRepository.AddItemAsync(novoItem);
                    }
                }
                else
                {
                    if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                    {
                        throw new Exception("_httpContextAccessor está NULO. Verifique 'AddHttpContextAccessor' no Program.cs.");
                    }
                    if (_httpContextAccessor.HttpContext.Session == null)
                    {
                        throw new Exception("A SESSÃO (Session) está NULA. Verifique 'app.UseSession()' no Program.cs.");
                    }

                    var session = _httpContextAccessor.HttpContext.Session;
                    var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();

                    var itemExistente = carrinhoSessao.FirstOrDefault(i => i.ProdutoId == produtoId);

                    if (itemExistente != null)
                    {
                        itemExistente.Quantidade += quantidade;
                    }
                    else
                    {
                        carrinhoSessao.Add(new ItemCarrinhoModel
                        {
                            ProdutoId = produtoId,
                            Quantidade = quantidade
                        });
                    }
                    session.SetObject("Carrinho", carrinhoSessao);
                }

                TempData["SucessoCarrinho"] = $"'{produto.Nome}' adicionado ao carrinho!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string erroReal = ex.Message;

                if (ex.InnerException != null)
                {
                    erroReal = ex.InnerException.Message;
                }

                TempData["ErroCarrinho"] = $"ERRO 500 (DbUpdate): {erroReal}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverDoCarrinho(int produtoId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                await _carrinhoRepository.RemoveItemAsync(produtoId, user.Id);
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho");
                if (carrinhoSessao != null)
                {
                    var itemParaRemover = carrinhoSessao.FirstOrDefault(i => i.ProdutoId == produtoId);
                    if (itemParaRemover != null)
                    {
                        carrinhoSessao.Remove(itemParaRemover);
                        session.SetObject("Carrinho", carrinhoSessao);
                    }
                }
            }

            TempData["SucessoCarrinho"] = "Item removido do carrinho.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarQuantidade(int produtoId, int novaQuantidade)
        {
            if (novaQuantidade <= 0)
            {
                return await RemoverDoCarrinho(produtoId);
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var itemExistente = await _carrinhoRepository.GetItemAsync(produtoId, user.Id);

                if (itemExistente != null)
                {
                    itemExistente.Quantidade = novaQuantidade;
                    await _carrinhoRepository.UpdateItemAsync(itemExistente);
                }
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho");
                if (carrinhoSessao != null)
                {
                    var itemExistente = carrinhoSessao.FirstOrDefault(i => i.ProdutoId == produtoId);
                    if (itemExistente != null)
                    {
                        itemExistente.Quantidade = novaQuantidade;
                        session.SetObject("Carrinho", carrinhoSessao);
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}