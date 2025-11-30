using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToughService.Models;
using ToughService.Extensions;
using ToughService.Repository;
using System.Dynamic; // Necessário para ExpandoObject

namespace ToughService.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChamadoRepository _chamadoRepository;
        private readonly IPedidoRepository _pedidoRepository; // Repositório de Pedidos injetado

        public ClienteController(
            IProdutoRepository produtoRepository,
            UserManager<ApplicationUser> userManager,
            IChamadoRepository chamadoRepository,
            IPedidoRepository pedidoRepository)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
            _chamadoRepository = chamadoRepository;
            _pedidoRepository = pedidoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> MeusPedidos()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Registro");
            }

            // 1. Busca os pedidos reais do banco de dados (garanta que o repositório use .Include)
            var pedidosDb = await _pedidoRepository.GetPedidosByUserIdAsync(user.Id);

            // 2. Converte para o formato dynamic que a View espera
            var pedidosList = new List<dynamic>();

            foreach (var pedido in pedidosDb.OrderByDescending(p => p.DataPedido))
            {
                var dynPedido = new ExpandoObject() as IDictionary<string, object>;

                dynPedido["PedidoId"] = pedido.Id;
                dynPedido["Data"] = pedido.DataPedido;
                dynPedido["Total"] = pedido.Total;

                // Define o status amigável
                dynPedido["Status"] = string.IsNullOrEmpty(pedido.MercadoPagoStatus)
                                      ? pedido.Status.ToString()
                                      : TraduzirStatusMP(pedido.MercadoPagoStatus, pedido.Status.ToString());

                // Passa o link de pagamento para o botão "Pagar Agora" (se houver)
                dynPedido["LinkPagamento"] = pedido.MercadoPagoPreferenceId;

                // Pega o primeiro item para ilustrar o card
                var primeiroItemDb = pedido.Itens.FirstOrDefault();

                if (primeiroItemDb != null)
                {
                    dynPedido["Item"] = new ItemCarrinhoModel
                    {
                        Produto = primeiroItemDb.Produto,
                        Quantidade = pedido.Itens.Sum(i => i.Quantidade),
                        PrecoUnitario = primeiroItemDb.PrecoUnitario,
                        Total = primeiroItemDb.Subtotal
                    };
                }
                else
                {
                    dynPedido["Item"] = null;
                }

                pedidosList.Add(dynPedido);
            }

            ViewBag.Pedidos = pedidosList;
            ViewBag.User = user;

            return View();
        }

        // Função auxiliar para traduzir status do Mercado Pago
        private string TraduzirStatusMP(string statusMp, string statusInterno)
        {
            return statusMp.ToLower() switch
            {
                "approved" => "Aprovado",
                "pending" => "Pendente",
                "in_process" => "Em Análise",
                "rejected" => "Recusado",
                "cancelled" => "Cancelado",
                _ => statusInterno
            };
        }

        [HttpGet]
        public async Task<IActionResult> MeusServicos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
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