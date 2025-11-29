using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models; 
using ToughService.Repository;
using ToughService.Extensions;
using ToughService.Services;

namespace ToughService.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IEmailService _emailService;
        public CarrinhoController(
            ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService) 
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
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

                foreach (var item in carrinho)
                {
                    if (item.Produto == null)
                    {
                        item.Produto = await _produtoRepository.GetProdutoByIdAsync(item.ProdutoId);
                    }
                }
            }

            return View(carrinho);
        }

        [Route("Carrinho/InfoProduto/{id}")]
        [HttpGet]
        public async Task<IActionResult> InfoProduto(int id)
        {
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
            var produto = await _produtoRepository.GetProdutoByIdAsync(produtoId);
            if (produto == null)
            {
                TempData["ErroCarrinho"] = "Produto não encontrado.";
                return RedirectToAction("Index");
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
                var session = _httpContextAccessor.HttpContext.Session;
                var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();

                var itemExistente = carrinhoSessao.FirstOrDefault(i => i.ProdutoId == produtoId);

                if (itemExistente != null)
                {
                    itemExistente.Quantidade += quantidade;
                }
                else
                {
                    // Usa o Modelo "Limpo"
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
        public async Task<IActionResult> AlterarQuantidade(int produtoId, int delta)
        {
            if (delta == 0)
            {
                return RedirectToAction("Index");
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var item = await _carrinhoRepository.GetItemAsync(produtoId, user.Id);
                if (item != null)
                {
                    item.Quantidade += delta;
                    if (item.Quantidade <= 0)
                    {
                        await _carrinhoRepository.RemoveItemAsync(produtoId, user.Id);
                    }
                    else
                    {
                        await _carrinhoRepository.UpdateItemAsync(item);
                    }
                }
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();
                var item = carrinhoSessao.FirstOrDefault(i => i.ProdutoId == produtoId);
                if (item != null)
                {
                    item.Quantidade += delta;
                    if (item.Quantidade <= 0)
                    {
                        carrinhoSessao.Remove(item);
                    }
                }
                session.SetObject("Carrinho", carrinhoSessao);
            }

            TempData["SucessoCarrinho"] = "Quantidade atualizada.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManterSomenteProduto(int produtoId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var carrinho = await _carrinhoRepository.GetCarrinhoByUserIdAsync(user.Id);

                foreach (var item in carrinho.Where(i => i.ProdutoId != produtoId).ToList())
                {
                    await _carrinhoRepository.RemoveItemAsync(item.ProdutoId, user.Id);
                }
            }
            else
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();
                carrinhoSessao = carrinhoSessao.Where(i => i.ProdutoId == produtoId).ToList();
                session.SetObject("Carrinho", carrinhoSessao);
            }

            TempData["SucessoCarrinho"] = "Mantido apenas o produto selecionado no carrinho.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCarrinho() 
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                await _carrinhoRepository.ClearCarrinhoAsync(user.Id); 
            }
            else
            {
                _httpContextAccessor.HttpContext.Session.Remove("Carrinho");
            }
            TempData["SucessoCarrinho"] = "Carrinho limpo com sucesso.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutSelecionado(string selectedProdutoIds)
        {
            var session = _httpContextAccessor.HttpContext.Session;

            if (!string.IsNullOrWhiteSpace(selectedProdutoIds))
            {
                var ids = selectedProdutoIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                session.SetObject("SelectedProdutoIds", ids);
            }

            return RedirectToAction("Index", "Checkout");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return RedirectToAction("Index", "Checkout");
        }

    }
}