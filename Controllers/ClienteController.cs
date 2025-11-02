using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToughService.Models;
using ToughService.Repository;

namespace ToughService.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClienteController(
            ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MeusPedidos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Registro");
            }

            // Buscar itens do carrinho do usuário (que podem ser considerados como pedidos/pedidos em andamento)
            var carrinho = await _carrinhoRepository.GetCarrinhoByUserIdAsync(userId);

            // Calcular totais para cada item
            foreach (var item in carrinho)
            {
                if (item.Produto == null && item.ProdutoId > 0)
                {
                    item.Produto = await _produtoRepository.GetProdutoByIdAsync(item.ProdutoId);
                }
            }

            // Preparar dados dos pedidos (simulação)
            // Em uma implementação futura, você pode criar uma tabela de Pedidos
            var pedidosList = new System.Collections.Generic.List<dynamic>();
            
            for (int i = 0; i < carrinho.Count; i++)
            {
                var item = carrinho[i];
                var produto = item.Produto;
                
                if (produto != null)
                {
                    var pedido = new System.Dynamic.ExpandoObject() as System.Collections.Generic.IDictionary<string, object>;
                    pedido["PedidoId"] = $"PED-{item.Id:D6}-{i}";
                    pedido["Item"] = item;
                    pedido["Data"] = System.DateTime.Now.AddDays(-i);
                    pedido["Total"] = produto.Preco * item.Quantidade;
                    
                    // Status variado para demonstração
                    if (i % 3 == 0)
                        pedido["Status"] = "Entregue";
                    else if (i % 3 == 1)
                        pedido["Status"] = "Em Trânsito";
                    else
                        pedido["Status"] = "Pendente";
                    
                    pedidosList.Add(pedido);
                }
            }

            ViewBag.Pedidos = pedidosList;
            ViewBag.User = await _userManager.GetUserAsync(User);

            return View(carrinho);
        }

        [HttpGet]
        public IActionResult Configuracoes()
        {
            // Implementação futura para configurações do cliente
            return RedirectToAction("Perfil", "Perfil");
        }
    }
}

