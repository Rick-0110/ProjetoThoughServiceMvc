using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Repository;
using ToughService.Services;

namespace ToughService.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IMercadoPagoService mercadoPagoService,
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<PaymentController> logger)
        {
            _mercadoPagoService = mercadoPagoService;
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePayment(int pedidoId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoId);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    return NotFound();
                }

                // Aqui você precisaria reconstruir o CheckoutViewModel a partir do pedido
                // Por enquanto, vamos redirecionar para o checkout
                return RedirectToAction("Index", "Checkout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar pagamento para pedido {pedidoId}");
                TempData["ErroPagamento"] = "Erro ao processar pagamento. Tente novamente.";
                return RedirectToAction("Index", "Checkout");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int pedidoId, string payment_id = null, string status = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoId);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(payment_id))
                {
                    // Atualizar informações do pagamento
                    pedido.MercadoPagoPaymentId = payment_id;
                    pedido.MercadoPagoStatus = status ?? "approved";
                    pedido.MercadoPagoPaymentDate = DateTime.Now;

                if (status == "approved")
                {
                    pedido.Status = StatusPedidoEnum.Confirmado;
                    // Limpar carrinho após pagamento aprovado
                    await _carrinhoRepository.ClearCarrinhoAsync(pedido.UserId);
                }

                await _pedidoRepository.UpdatePedidoAsync(pedido);
                }

                TempData["SucessoPagamento"] = "Pagamento processado com sucesso!";
                return RedirectToAction("Confirmation", "Checkout", new { id = pedidoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar sucesso do pagamento para pedido {pedidoId}");
                TempData["ErroPagamento"] = "Erro ao processar confirmação do pagamento.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Failure(int pedidoId, string payment_id = null, string status = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoId);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(payment_id))
                {
                    pedido.MercadoPagoPaymentId = payment_id;
                    pedido.MercadoPagoStatus = status ?? "rejected";
                    await _pedidoRepository.UpdatePedidoAsync(pedido);
                }

                TempData["ErroPagamento"] = "O pagamento não foi aprovado. Tente novamente ou escolha outra forma de pagamento.";
                return RedirectToAction("Index", "Checkout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar falha do pagamento para pedido {pedidoId}");
                TempData["ErroPagamento"] = "Erro ao processar pagamento.";
                return RedirectToAction("Index", "Checkout");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pending(int pedidoId, string payment_id = null, string status = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoId);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(payment_id))
                {
                    pedido.MercadoPagoPaymentId = payment_id;
                    pedido.MercadoPagoStatus = status ?? "pending";
                    pedido.Status = StatusPedidoEnum.Pendente;
                    await _pedidoRepository.UpdatePedidoAsync(pedido);
                }

                TempData["InfoPagamento"] = "Seu pagamento está sendo processado. Você receberá uma confirmação por e-mail quando for aprovado.";
                return RedirectToAction("Confirmation", "Checkout", new { id = pedidoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar pagamento pendente para pedido {pedidoId}");
                TempData["ErroPagamento"] = "Erro ao processar pagamento.";
                return RedirectToAction("Index", "Checkout");
            }
        }
    }
}

