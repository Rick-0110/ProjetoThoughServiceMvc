using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ToughService.Models;
using ToughService.Repository;
using ToughService.Services;

namespace ToughService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IMercadoPagoService mercadoPagoService,
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            ILogger<WebhookController> logger)
        {
            _mercadoPagoService = mercadoPagoService;
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _logger = logger;
        }

        [HttpPost("MercadoPago")]
        [HttpGet("MercadoPago")]
        public async Task<IActionResult> MercadoPagoWebhook()
        {
            try
            {
                string? type = null;
                string? dataId = null;

                // O Mercado Pago pode enviar notificações via query parameters ou body
                // Primeiro, tenta obter dos query parameters
                if (Request.Query.ContainsKey("type"))
                {
                    type = Request.Query["type"].ToString();
                }

                if (Request.Query.ContainsKey("data_id"))
                {
                    dataId = Request.Query["data_id"].ToString();
                }

                // Se não encontrou nos query parameters, tenta ler do body
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(dataId))
                {
                    if (Request.ContentLength > 0 || Request.ContentLength == null)
                    {
                        try
                        {
                            Request.EnableBuffering();
                            Request.Body.Position = 0;
                            
                            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
                            var body = await reader.ReadToEndAsync();
                            Request.Body.Position = 0;

                            _logger.LogInformation($"Webhook recebido do Mercado Pago (body): {body}");

                            if (!string.IsNullOrEmpty(body))
                            {
                                try
                                {
                                    var notification = JsonSerializer.Deserialize<MercadoPagoNotification>(body, new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    });

                                    if (notification != null)
                                    {
                                        type = notification.Type ?? type;
                                        dataId = notification.Data?.Id ?? dataId;
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    _logger.LogWarning(ex, "Erro ao deserializar notificação JSON do Mercado Pago");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Erro ao ler body da requisição");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Webhook recebido do Mercado Pago (query): type={type}, data_id={dataId}");
                }

                // Processar diferentes tipos de notificação
                if (type == "payment" && !string.IsNullOrEmpty(dataId))
                {
                    await ProcessPaymentNotification(dataId);
                }
                else if (type == "merchant_order" && !string.IsNullOrEmpty(dataId))
                {
                    _logger.LogInformation($"Notificação de ordem recebida: {dataId}");
                    // Processar notificação de ordem se necessário
                }
                else
                {
                    _logger.LogWarning($"Tipo de notificação desconhecido ou dados inválidos: type={type}, data_id={dataId}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook do Mercado Pago");
                return StatusCode(500, "Erro ao processar webhook");
            }
        }

        private async Task ProcessPaymentNotification(string paymentId)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentId))
                {
                    _logger.LogWarning("ID do pagamento não fornecido na notificação");
                    return;
                }

                _logger.LogInformation($"Processando notificação de pagamento: {paymentId}");

                var paymentStatus = await _mercadoPagoService.GetPaymentStatusAsync(paymentId);

                if (paymentStatus == null)
                {
                    _logger.LogWarning($"Status do pagamento {paymentId} não pôde ser obtido");
                    return;
                }

                if (string.IsNullOrEmpty(paymentStatus.OrderId))
                {
                    _logger.LogWarning($"Pedido não encontrado para o pagamento {paymentId}");
                    return;
                }

                if (!int.TryParse(paymentStatus.OrderId, out int pedidoId))
                {
                    _logger.LogWarning($"ID do pedido inválido: {paymentStatus.OrderId}");
                    return;
                }

                var pedido = await _pedidoRepository.GetPedidoByIdAsync(pedidoId);
                if (pedido == null)
                {
                    _logger.LogWarning($"Pedido {pedidoId} não encontrado");
                    return;
                }

                // Atualizar informações do pagamento no pedido
                pedido.MercadoPagoPaymentId = paymentId;
                pedido.MercadoPagoStatus = paymentStatus.Status ?? string.Empty;
                pedido.MercadoPagoPaymentDate = DateTime.Now;

                // Atualizar status do pedido baseado no status do pagamento
                var statusLower = paymentStatus.Status?.ToLower() ?? string.Empty;
                switch (statusLower)
                {
                    case "approved":
                        pedido.Status = StatusPedidoEnum.Confirmado;
                        // Limpar carrinho após pagamento aprovado
                        await _carrinhoRepository.ClearCarrinhoAsync(pedido.UserId);
                        _logger.LogInformation($"Pagamento {paymentId} aprovado. Pedido {pedidoId} confirmado. Carrinho limpo.");
                        break;
                    case "rejected":
                    case "cancelled":
                        pedido.Status = StatusPedidoEnum.Cancelado;
                        _logger.LogInformation($"Pagamento {paymentId} rejeitado/cancelado. Pedido {pedidoId} cancelado.");
                        break;
                    case "pending":
                    case "in_process":
                        pedido.Status = StatusPedidoEnum.Pendente;
                        _logger.LogInformation($"Pagamento {paymentId} pendente. Pedido {pedidoId} mantido como pendente.");
                        break;
                    default:
                        _logger.LogWarning($"Status de pagamento desconhecido: {paymentStatus.Status}");
                        break;
                }

                await _pedidoRepository.UpdatePedidoAsync(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar notificação de pagamento {paymentId}");
            }
        }
    }

    // Classes para deserializar a notificação do Mercado Pago
    // Usando PropertyNameCaseInsensitive = true, não precisamos de JsonPropertyName
    public class MercadoPagoNotification
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Action { get; set; }
        public MercadoPagoNotificationData? Data { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class MercadoPagoNotificationData
    {
        public string? Id { get; set; }
    }
}
