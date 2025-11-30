using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ToughService.Models;
using ToughService.Models.ModelCheckout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToughService.Services
{
    // A interface que você mandou fica aqui (ou em arquivo separado, mas o namespace deve bater)
    // Se já estiver em outro arquivo, pode remover este bloco interface daqui.

    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MercadoPagoService> _logger;

        public MercadoPagoService(IConfiguration configuration, ILogger<MercadoPagoService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Método auxiliar para garantir que o Token esteja carregado antes de qualquer chamada
        private void ConfigurarSDK()
        {
            // Tenta pegar o token da chave correta
            var accessToken = _configuration["MercadoPago:AccessToken"];

            if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("SEU_ACCESS_TOKEN"))
            {
                // Fallback para nome antigo se houver
                accessToken = _configuration["MercadoPagoSettings:AccessToken"];
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("AccessToken do Mercado Pago não encontrado. Configure usando 'dotnet user-secrets set \"MercadoPago:AccessToken\" \"SEU_TOKEN\"'");
            }

            MercadoPagoConfig.AccessToken = accessToken;
        }

        public async Task<string> CreatePreferenceAsync(CheckoutViewModel checkout, int pedidoId, string userId)
        {
            try
            {
                // 1. Configura o SDK
                ConfigurarSDK();

                _logger.LogInformation($"Criando preferência MP para pedido #{pedidoId}...");

                // 2. Monta a lista de itens
                var items = checkout.CartItems.Select(item => new PreferenceItemRequest
                {
                    Id = item.ProdutoId.ToString(),
                    Title = item.Nome,
                    Quantity = item.Quantidade,
                    CurrencyId = "BRL",
                    UnitPrice = item.PrecoUnitario
                }).ToList();

                // Adiciona Frete se houver
                if (checkout.ShippingCost > 0)
                {
                    items.Add(new PreferenceItemRequest
                    {
                        Id = "Frete",
                        Title = "Custo de Envio",
                        Quantity = 1,
                        CurrencyId = "BRL",
                        UnitPrice = checkout.ShippingCost
                    });
                }

                // 3. Monta a requisição completa
                var request = new PreferenceRequest
                {
                    Items = items,
                    Payer = new PreferencePayerRequest
                    {
                        Name = checkout.CheckoutName.Split(' ')[0],
                        Surname = checkout.CheckoutName.Contains(" ") ? checkout.CheckoutName.Substring(checkout.CheckoutName.IndexOf(" ") + 1) : "",
                        Email = checkout.CheckoutEmail,

                        // Telefone é opcional, adicione apenas se tiver certeza do formato
                        /* Phone = new PhoneRequest {
                            AreaCode = "11",
                            Number = "999999999"
                        }, */
                    },
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = $"{GetBaseUrl()}/Checkout/Confirmation?id={pedidoId}",
                        Failure = $"{GetBaseUrl()}/Checkout/Index",
                        Pending = $"{GetBaseUrl()}/Checkout/Confirmation?id={pedidoId}"
                    },
                    AutoReturn = "approved",
                    ExternalReference = pedidoId.ToString(), // VITAL para o Webhook saber qual pedido é
                    StatementDescriptor = "TOUGH SERVICE",
                    Expires = true,
                    ExpirationDateFrom = DateTime.Now,
                    ExpirationDateTo = DateTime.Now.AddDays(1)
                };

                // 4. Envia para o Mercado Pago
                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(request);

                if (preference == null)
                    throw new Exception("MP retornou preferência nula.");

                // Retorna o link (InitPoint)
                // Use SandboxInitPoint se estiver usando credenciais de teste, ou InitPoint para produção
                // O MP decide automático baseado no Token, mas o InitPoint geralmente serve para ambos
                var link = preference.InitPoint;

                return link;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar preferência para pedido {pedidoId}");
                throw;
            }
        }

        public async Task<bool> ProcessPaymentNotificationAsync(string paymentId)
        {
            try
            {
                ConfigurarSDK();
                var client = new PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId));

                return payment != null && payment.Status == "approved";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar notificação {paymentId}");
                return false;
            }
        }

        public async Task<MercadoPagoPaymentStatus> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                ConfigurarSDK();
                var client = new PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId));

                if (payment == null) return null;

                return new MercadoPagoPaymentStatus
                {
                    Status = payment.Status,
                    PaymentId = payment.Id.ToString(),
                    OrderId = payment.ExternalReference,
                    Amount = payment.TransactionAmount ?? 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao consultar pagamento {paymentId}");
                return null;
            }
        }

        private string GetBaseUrl()
        {
            // Em produção, isso deve vir do appsettings. Em dev, usa localhost.
            var url = _configuration["BaseUrl"];
            return string.IsNullOrEmpty(url) ? "https://localhost:7004" : url;
        }
    }
}