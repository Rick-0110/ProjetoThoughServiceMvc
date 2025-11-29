using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToughService.Models;
using ToughService.Extensions;
using ToughService.Repository;

namespace ToughService.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChamadoRepository _chamadoRepository;

        public ClienteController(
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager,
            IChamadoRepository chamadoRepository)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
            _chamadoRepository = chamadoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> MeusPedidos()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Registro");
            }

            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();

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
                    pedido["Total"] = item.Produto.Preco * item.Quantidade;
                    
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
        public async Task<IActionResult> MeusServicos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var chamados = await _chamadoRepository.GetChamadosByUserIdAsync(userId);
            return View(chamados);

        }


        [HttpGet]
        public IActionResult Configuracoes()
        {
            return RedirectToAction("Perfil", "Perfil");
        }
    }
}

