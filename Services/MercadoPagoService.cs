using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using ToughService.Models;
using ToughService.Models.ModelCheckout;

namespace ToughService.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MercadoPagoService> _logger;

        public MercadoPagoService(IConfiguration configuration, ILogger<MercadoPagoService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Configurar o SDK do Mercado Pago
            var accessToken = _configuration["MercadoPagoSettings:AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("AccessToken do Mercado Pago não configurado");
            }
            else
            {
                MercadoPagoConfig.AccessToken = accessToken;
            }
        }

        public async Task<string> CreatePreferenceAsync(CheckoutViewModel checkout, int pedidoId, string userId)
        {
            try
            {
                var accessToken = _configuration["MercadoPagoSettings:AccessToken"];
                if (string.IsNullOrEmpty(accessToken) || accessToken == "SEU_ACCESS_TOKEN_PRIVADO_AQUI")
                {
                    _logger.LogError("AccessToken do Mercado Pago não configurado corretamente no appsettings.json");
                    throw new InvalidOperationException("AccessToken do Mercado Pago não configurado. Configure no appsettings.json");
                }

                _logger.LogInformation($"Criando preferência para pedido {pedidoId} com {checkout.CartItems.Count} itens");
                
                var request = new PreferenceRequest
                {
                    Items = checkout.CartItems.Select(item => new MercadoPago.Client.Preference.PreferenceItemRequest
                    {
                        Title = item.Nome,
                        Quantity = item.Quantidade,
                        UnitPrice = item.PrecoUnitario,
                        CurrencyId = "BRL"
                    }).ToList(),
                    Payer = new MercadoPago.Client.Preference.PreferencePayerRequest
                    {
                        Name = checkout.CheckoutName,
                        Email = checkout.CheckoutEmail,
                    Phone = !string.IsNullOrEmpty(checkout.CheckoutPhone) && checkout.CheckoutPhone.Length > 2
                        ? new MercadoPago.Client.Common.PhoneRequest
                        {
                            AreaCode = checkout.CheckoutPhone.Substring(0, Math.Min(2, checkout.CheckoutPhone.Length)),
                            Number = checkout.CheckoutPhone.Substring(Math.Min(2, checkout.CheckoutPhone.Length))
                        }
                        : null
                    },
                    BackUrls = new MercadoPago.Client.Preference.PreferenceBackUrlsRequest
                    {
                        Success = $"{_configuration["BaseUrl"] ?? "https://localhost:5001"}/Payment/Success?pedidoId={pedidoId}",
                        Failure = $"{_configuration["BaseUrl"] ?? "https://localhost:5001"}/Payment/Failure?pedidoId={pedidoId}",
                        Pending = $"{_configuration["BaseUrl"] ?? "https://localhost:5001"}/Payment/Pending?pedidoId={pedidoId}"
                    },
                    AutoReturn = "approved",
                    ExternalReference = pedidoId.ToString(),
                    NotificationUrl = $"{_configuration["BaseUrl"] ?? "https://localhost:5001"}/api/Webhook/MercadoPago",
                    StatementDescriptor = "Tough Service",
                    AdditionalInfo = $"Pedido #{pedidoId} - {checkout.CheckoutName}"
                };

                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(request);

                if (preference == null)
                {
                    _logger.LogError($"Resposta do Mercado Pago está vazia para pedido {pedidoId}");
                    throw new InvalidOperationException("Não foi possível criar a preferência de pagamento");
                }

                _logger.LogInformation($"Preferência criada com sucesso. ID: {preference.Id}, Pedido: {pedidoId}");

                // O InitPoint contém a URL para redirecionar o usuário ao checkout
                // Em produção: preference.InitPoint
                // Em sandbox: preference.SandboxInitPoint
                var initPoint = preference.InitPoint ?? preference.SandboxInitPoint;
                
                if (string.IsNullOrEmpty(initPoint))
                {
                    _logger.LogError($"InitPoint não retornado pelo Mercado Pago. Preference ID: {preference.Id}");
                    throw new InvalidOperationException("URL de checkout não foi gerada pelo Mercado Pago");
                }

                _logger.LogInformation($"InitPoint obtido: {initPoint}");
                return initPoint;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar preferência do Mercado Pago para o pedido {pedidoId}");
                throw;
            }
        }

        public async Task<bool> ProcessPaymentNotificationAsync(string paymentId)
        {
            try
            {
                var client = new MercadoPago.Client.Payment.PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId));

                if (payment == null)
                {
                    _logger.LogWarning($"Pagamento {paymentId} não encontrado");
                    return false;
                }

                _logger.LogInformation($"Processando notificação de pagamento {paymentId}. Status: {payment.Status}");

                return payment.Status == "approved";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar notificação de pagamento {paymentId}");
                return false;
            }
        }

        public async Task<MercadoPagoPaymentStatus> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var client = new MercadoPago.Client.Payment.PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId));

                if (payment == null)
                {
                    return new MercadoPagoPaymentStatus
                    {
                        Status = "not_found",
                        PaymentId = paymentId
                    };
                }

                return new MercadoPagoPaymentStatus
                {
                    Status = payment.Status ?? "unknown",
                    PaymentId = paymentId,
                    OrderId = payment.ExternalReference ?? string.Empty,
                    Amount = payment.TransactionAmount ?? 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter status do pagamento {paymentId}");
                return new MercadoPagoPaymentStatus
                {
                    Status = "error",
                    PaymentId = paymentId
                };
            }
        }
    }
}

