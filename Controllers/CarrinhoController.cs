using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToughService.Data;
using ToughService.Models;
using ToughService.Extensions;
using ToughService.Repository;

namespace ToughService.Controllers
{
    
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarrinhoController(
            IProdutoRepository produtoRepository, 
            ICarrinhoRepository carrinhoRepository,
            UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _carrinhoRepository = carrinhoRepository;
            _userManager = userManager;
        }


    

        
        private const string CarrinhoSessionKey = "Carrinho";

        
        public IActionResult Index()
        {
            
           var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

           
            var subtotal = 0m;
            foreach (var item in carrinho)
            {
                subtotal += item.Preco * item.Quantidade;
            }

            var frete = 0m;

            if (subtotal > 100)
            {
                frete = 0; 
            }
            else
            {
                frete = 10; 
            }
            var total = subtotal + frete;

            ViewBag.Subtotal = subtotal;
            ViewBag.Frete = frete;
            ViewBag.Total = total;

            return View(carrinho);
        }




     



        [HttpPost]
        public async Task<IActionResult> Adicionar(int id, int quantidade)
        {
                 if (!User.Identity.IsAuthenticated)
            {
        return RedirectToAction("Login", "Registro");
    }

            var produto = await _produtoRepository.GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

            var itemExistente = carrinho.FirstOrDefault(i => i.Id == id);
            int quantidadeFinal;
            
            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
                quantidadeFinal = itemExistente.Quantidade;
            }
            else
            {
                carrinho.Add(new ItemCarrinhoModel
                {
                    Id = produto.Id,
                    NomeProduto = produto.Nome,
                    Preco = produto.Preco,
                    Quantidade = quantidade,
                    ImagemUrl = produto.ImagemUrl
                });
                quantidadeFinal = quantidade;
            }

            HttpContext.Session.SetObject(CarrinhoSessionKey, carrinho);

            // Salvar também no banco de dados se o usuário estiver autenticado
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    // Buscar item existente no banco
                    var itensBanco = await _carrinhoRepository.ObterItensPorUsuarioAsync(userId);
                    var itemBanco = itensBanco.FirstOrDefault(i => i.ProdutoId == id);
                    
                    if (itemBanco != null)
                    {
                        // Atualizar quantidade no banco para corresponder à sessão
                        await _carrinhoRepository.AtualizarQuantidadeAsync(itemBanco.Id, quantidadeFinal);
                    }
                    else
                    {
                        // Adicionar novo item no banco
                        var carrinhoItem = new CarrinhoItem
                        {
                            UserId = userId,
                            ProdutoId = produto.Id,
                            NomeProduto = produto.Nome,
                            Preco = produto.Preco,
                            Quantidade = quantidadeFinal,
                            ImagemUrl = produto.ImagemUrl,
                            DataAdicionado = DateTime.Now
                        };
                        
                        await _carrinhoRepository.AdicionarItemAsync(carrinhoItem);
                    }
                }
            }

            return RedirectToAction("Index");
       
}


        public async Task<IActionResult> InfoProduto(int id)
        {
         
            var produto = await _produtoRepository.GetProdutoByIdAsync(id);

            if (produto == null)
                return NotFound();

            var todosProdutos = await _produtoRepository.GetAllProdutosAsync();
            var outrosProdutos = todosProdutos
                .Where(p => p.Id != id)
                .Take(4)
                .ToList();

            var viewModel = new ProdutoDetalheViewModel
            {
                Produto = produto,
                OutrosProdutos = outrosProdutos
            };

            return View(viewModel);
        }






        [HttpPost]
        public async Task<IActionResult> Limpar()
        {
            // Limpar da sessão
            HttpContext.Session.Remove(CarrinhoSessionKey);

            // Limpar também do banco de dados se o usuário estiver autenticado
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    await _carrinhoRepository.RemoverItensPorUsuarioAsync(userId);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
