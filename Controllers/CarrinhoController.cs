using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models; // <-- Verifique o namespace dos seus Modelos
using ToughService.Models;          // <-- Verifique se ItemCarrinhoModel está aqui
using ToughService.Repository;    // <-- Para os Repositórios
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            // Calcular o Total (para mostrar na View)
            decimal total = 0;
            if (carrinho != null && carrinho.Any())
            {
                total = carrinho.Sum(item => (item.Produto?.Preco ?? 0) * item.Quantidade);
            }
            ViewBag.TotalCarrinho = total;

            return View(carrinho);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarAoCarrinho(int produtoId, int quantidade = 1)
        {
            var produto = await _produtoRepository.GetProdutoByIdAsync(produtoId);
            if (produto == null)
            {
                TempData["ErroCarrinho"] = "Produto não encontrado.";
                return Redirect(Request.Headers["Referer"].ToString() ?? "/"); 
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var itemExistente = await _carrinhoRepository.GetItemAsync(produtoId, user.Id);

                if (itemExistente != null)
                {
                    itemExistente.Quantidade += quantidade;
                    await _carrinhoRepository.UpdateItemAsync(itemExistente);
                }
                else
                {
                    // Se não existe, cria novo
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
                // === ANÓNIMO: Lógica da Session ===
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