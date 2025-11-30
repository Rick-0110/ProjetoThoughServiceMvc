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
        public async Task<IActionResult> Success(
            int? pedidoId = null,
            string payment_id = null,
            string status = null,
            string external_reference = null,
            string collection_id = null,
            string collection_status = null,
            string merchant_order_id = null,
            string preference_id = null)
        {
            try
            {
                _logger.LogInformation($"Success chamado. Parâmetros: payment_id={payment_id}, status={status}, external_reference={external_reference}, pedidoId={pedidoId}");

                // Usar external_reference como fonte primária para identificar o pedido
                int pedidoIdFinal = pedidoId ?? 0;
                if (!string.IsNullOrEmpty(external_reference) && int.TryParse(external_reference, out int refPedidoId))
                {
                    pedidoIdFinal = refPedidoId;
                }

                if (pedidoIdFinal == 0)
                {
                    _logger.LogWarning("Não foi possível identificar o pedido. Parâmetros recebidos não contêm pedidoId ou external_reference válido.");
                    TempData["ErroPagamento"] = "Não foi possível identificar o pedido.";
                    return RedirectToAction("Index", "Home");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoIdFinal);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    _logger.LogWarning($"Pedido {pedidoIdFinal} não encontrado ou não pertence ao usuário {user.Id}");
                    return NotFound();
                }

                // Atualizar informações do pagamento
                if (!string.IsNullOrEmpty(payment_id))
                {
                    pedido.MercadoPagoPaymentId = payment_id;
                }
                else if (!string.IsNullOrEmpty(collection_id))
                {
                    pedido.MercadoPagoPaymentId = collection_id;
                }

                pedido.MercadoPagoStatus = status ?? collection_status ?? "approved";
                pedido.MercadoPagoPaymentDate = DateTime.Now;

                if (status == "approved" || collection_status == "approved")
                {
                    pedido.Status = StatusPedidoEnum.Confirmado;
                    // Limpar carrinho após pagamento aprovado
                    await _carrinhoRepository.ClearCarrinhoAsync(pedido.UserId);
                    _logger.LogInformation($"Pagamento aprovado. Pedido {pedidoIdFinal} confirmado e carrinho limpo.");
                }

                await _pedidoRepository.UpdatePedidoAsync(pedido);

                TempData["SucessoPagamento"] = "Pagamento processado com sucesso!";
                return RedirectToAction("Confirmation", "Checkout", new { id = pedidoIdFinal });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar sucesso do pagamento. pedidoId={pedidoId}, external_reference={external_reference}");
                TempData["ErroPagamento"] = "Erro ao processar confirmação do pagamento.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Failure(
            int? pedidoId = null,
            string payment_id = null,
            string status = null,
            string external_reference = null,
            string collection_id = null,
            string collection_status = null,
            string merchant_order_id = null)
        {
            try
            {
                _logger.LogInformation($"Failure chamado. Parâmetros: payment_id={payment_id}, status={status}, external_reference={external_reference}, pedidoId={pedidoId}");

                // Usar external_reference como fonte primária para identificar o pedido
                int pedidoIdFinal = pedidoId ?? 0;
                if (!string.IsNullOrEmpty(external_reference) && int.TryParse(external_reference, out int refPedidoId))
                {
                    pedidoIdFinal = refPedidoId;
                }

                if (pedidoIdFinal == 0)
                {
                    _logger.LogWarning("Não foi possível identificar o pedido no Failure.");
                    TempData["ErroPagamento"] = "O pagamento não foi aprovado. Tente novamente ou escolha outra forma de pagamento.";
                    return RedirectToAction("Index", "Checkout");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoIdFinal);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    _logger.LogWarning($"Pedido {pedidoIdFinal} não encontrado no Failure.");
                    TempData["ErroPagamento"] = "O pagamento não foi aprovado. Tente novamente ou escolha outra forma de pagamento.";
                    return RedirectToAction("Index", "Checkout");
                }

                // Atualizar informações do pagamento
                if (!string.IsNullOrEmpty(payment_id))
                {
                    pedido.MercadoPagoPaymentId = payment_id;
                }
                else if (!string.IsNullOrEmpty(collection_id))
                {
                    pedido.MercadoPagoPaymentId = collection_id;
                }

                pedido.MercadoPagoStatus = status ?? collection_status ?? "rejected";
                pedido.MercadoPagoPaymentDate = DateTime.Now;
                
                if (status == "rejected" || collection_status == "rejected" || status == "cancelled" || collection_status == "cancelled")
                {
                    pedido.Status = StatusPedidoEnum.Cancelado;
                }

                await _pedidoRepository.UpdatePedidoAsync(pedido);

                TempData["ErroPagamento"] = "O pagamento não foi aprovado. Tente novamente ou escolha outra forma de pagamento.";
                return RedirectToAction("Index", "Checkout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar falha do pagamento. pedidoId={pedidoId}, external_reference={external_reference}");
                TempData["ErroPagamento"] = "Erro ao processar pagamento.";
                return RedirectToAction("Index", "Checkout");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pending(
            int? pedidoId = null,
            string payment_id = null,
            string status = null,
            string external_reference = null,
            string collection_id = null,
            string collection_status = null,
            string merchant_order_id = null)
        {
            try
            {
                _logger.LogInformation($"Pending chamado. Parâmetros: payment_id={payment_id}, status={status}, external_reference={external_reference}, pedidoId={pedidoId}");

                // Usar external_reference como fonte primária para identificar o pedido
                int pedidoIdFinal = pedidoId ?? 0;
                if (!string.IsNullOrEmpty(external_reference) && int.TryParse(external_reference, out int refPedidoId))
                {
                    pedidoIdFinal = refPedidoId;
                }

                if (pedidoIdFinal == 0)
                {
                    _logger.LogWarning("Não foi possível identificar o pedido no Pending.");
                    TempData["InfoPagamento"] = "Seu pagamento está sendo processado. Você receberá uma confirmação por e-mail quando for aprovado.";
                    return RedirectToAction("Index", "Home");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoIdFinal);
                if (pedido == null || pedido.UserId != user.Id)
                {
                    _logger.LogWarning($"Pedido {pedidoIdFinal} não encontrado no Pending.");
                    TempData["InfoPagamento"] = "Seu pagamento está sendo processado. Você receberá uma confirmação por e-mail quando for aprovado.";
                    return RedirectToAction("Index", "Home");
                }

                // Atualizar informações do pagamento
                if (!string.IsNullOrEmpty(payment_id))
                {
                    pedido.MercadoPagoPaymentId = payment_id;
                }
                else if (!string.IsNullOrEmpty(collection_id))
                {
                    pedido.MercadoPagoPaymentId = collection_id;
                }

                pedido.MercadoPagoStatus = status ?? collection_status ?? "pending";
                pedido.MercadoPagoPaymentDate = DateTime.Now;
                pedido.Status = StatusPedidoEnum.Pendente;
                
                await _pedidoRepository.UpdatePedidoAsync(pedido);

                TempData["InfoPagamento"] = "Seu pagamento está sendo processado. Você receberá uma confirmação por e-mail quando for aprovado.";
                return RedirectToAction("Confirmation", "Checkout", new { id = pedidoIdFinal });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar pagamento pendente. pedidoId={pedidoId}, external_reference={external_reference}");
                TempData["ErroPagamento"] = "Erro ao processar pagamento.";
                return RedirectToAction("Index", "Checkout");
            }
        }
    }
}

