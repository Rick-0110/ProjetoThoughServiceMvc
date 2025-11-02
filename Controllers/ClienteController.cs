using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProjetoThoughServiceMvc.Models;
using ToughService.Extensions;
using ToughService.Repository;

namespace ToughService.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClienteController(
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MeusPedidos()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Registro");
            }

            // Buscar itens do carrinho da sessão
            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();

            // Preparar dados dos pedidos (simulação)
            // Em uma implementação futura, você pode criar uma tabela de Pedidos
            var pedidosList = new System.Collections.Generic.List<dynamic>();
            
            for (int i = 0; i < carrinho.Count; i++)
            {
                var item = carrinho[i];
                
                if (item != null)
                {
                    var pedido = new System.Dynamic.ExpandoObject() as System.Collections.Generic.IDictionary<string, object>;
                    pedido["PedidoId"] = $"PED-{item.Id:D6}-{i}";
                    pedido["Item"] = item;
                    pedido["Data"] = System.DateTime.Now.AddDays(-i);
                    pedido["Total"] = item.Preco * item.Quantidade;
                    
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

