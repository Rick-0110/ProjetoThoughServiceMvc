using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ToughService.Models;
using ToughService.Models.ModelCheckout;
using ToughService.Repository;
using ToughService.Services;

namespace ToughService.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICheckoutViewModelBuilder _checkoutViewModelBuilder;
        private readonly IEmailService _emailService;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CheckoutController> _logger;
        private readonly IWhatsAppService _whatsAppService; // Serviço de WhatsApp adicionado

        public CheckoutController(
            ICheckoutViewModelBuilder checkoutViewModelBuilder,
            IEmailService emailService,
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            IMercadoPagoService mercadoPagoService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ILogger<CheckoutController> logger,
            IWhatsAppService whatsAppService) // Injeção do WhatsApp
        {
            _checkoutViewModelBuilder = checkoutViewModelBuilder;
            _emailService = emailService;
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _mercadoPagoService = mercadoPagoService;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _whatsAppService = whatsAppService;
        }

        [HttpGet]
        [Route("[controller]")]
        [Route("Carrinho/Checkout")]
        public async Task<IActionResult> Index()
        {
            var model = await _checkoutViewModelBuilder.BuildAsync();

            if (model.CartItems == null || !model.CartItems.Any())
            {
                TempData["ErroCarrinho"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            // Passar a PublicKey para o Front (caso use scripts do MP)
            ViewData["MercadoPagoPublicKey"] = _configuration["MercadoPagoSettings:PublicKey"] ?? string.Empty;

            return View("~/Views/Carrinho/Checkout.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(CheckoutViewModel model)
        {
            _logger.LogInformation($"ConfirmOrder chamado. PaymentMethod: {model.PaymentMethod}");

            // 1. Reconstrói o modelo com dados do banco (para segurança de preços)
            var hydratedModel = await _checkoutViewModelBuilder.BuildAsync(model);

            if (hydratedModel.CartItems == null || !hydratedModel.CartItems.Any())
            {
                TempData["ErroCarrinho"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            // 2. Atualiza dados do formulário
            hydratedModel.CheckoutName = model.CheckoutName;
            hydratedModel.CheckoutEmail = model.CheckoutEmail;
            hydratedModel.CheckoutPhone = model.CheckoutPhone;
            hydratedModel.CheckoutCep = model.CheckoutCep;
            hydratedModel.CheckoutCidade = model.CheckoutCidade;
            hydratedModel.CheckoutEstado = model.CheckoutEstado;
            hydratedModel.CheckoutEndereco = model.CheckoutEndereco;
            hydratedModel.CheckoutNumero = model.CheckoutNumero;
            hydratedModel.CheckoutComplemento = model.CheckoutComplemento;
            hydratedModel.ShippingMethod = model.ShippingMethod;
            hydratedModel.PaymentMethod = model.PaymentMethod;
            hydratedModel.Total = model.Total;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido no checkout.");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            // Validação extra de segurança do total
            if (Math.Abs(hydratedModel.Total - model.Total) > 0.01m)
            {
                ModelState.AddModelError(string.Empty, "Erro de cálculo no total. O pedido não pode ser processado.");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            var user = await _userManager.GetUserAsync(User);

            // 3. Cria o Objeto Pedido
            var novoPedido = new PedidoModel
            {
                UserId = user.Id,
                DataPedido = DateTime.Now,
                Status = StatusPedidoEnum.Pendente,

                NomeCliente = model.CheckoutName,
                EmailCliente = model.CheckoutEmail,
                TelefoneCliente = model.CheckoutPhone,
                Cep = model.CheckoutCep,
                Logradouro = model.CheckoutEndereco,
                Numero = model.CheckoutNumero,
                Complemento = model.CheckoutComplemento,
                Bairro = "Não informado",
                Cidade = model.CheckoutCidade,
                Estado = model.CheckoutEstado,

                Subtotal = hydratedModel.Subtotal,
                CustoEnvio = hydratedModel.ShippingCost,
                Desconto = hydratedModel.Discount,
                Total = hydratedModel.Total,

                MetodoEnvio = model.ShippingMethod,
                MetodoPagamento = model.PaymentMethod
            };

            // Adiciona itens
            foreach (var item in hydratedModel.CartItems)
            {
                novoPedido.Itens.Add(new PedidoItemModel
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario,
                    Subtotal = item.Total
                });
            }

            // 4. Salva no Banco de Dados
            await _pedidoRepository.AddPedidoAsync(novoPedido);

            // ==================================================================
            // 5. NOTIFICAÇÃO WHATSAPP (Rodando em Background)
            // ==================================================================
            _ = Task.Run(async () =>
            {
                try
                {
                    var msg = $"🔔 *NOVO PEDIDO NO SITE!*\n\n" +
                              $"🛒 Pedido: #{novoPedido.Id}\n" +
                              $"👤 Cliente: {novoPedido.NomeCliente}\n" +
                              $"📞 Tel: {novoPedido.TelefoneCliente}\n" +
                              $"💰 Valor: R$ {novoPedido.Total:F2}\n" +
                              $"📦 Pagamento: {novoPedido.MetodoPagamento}";

                    await _whatsAppService.EnviarMensagemAsync(msg);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao enviar WhatsApp: {ex.Message}");
                }
            });

            // ==================================================================
            // 6. INTEGRAÇÃO MERCADO PAGO
            // ==================================================================
            try
            {
                _logger.LogInformation($"Criando preferência MP para pedido {novoPedido.Id}");

                // Cria o link de pagamento
                var initPoint = await _mercadoPagoService.CreatePreferenceAsync(hydratedModel, novoPedido.Id, user.Id);

                if (string.IsNullOrEmpty(initPoint))
                {
                    throw new Exception("Link de pagamento não gerado (InitPoint vazio).");
                }

                // Atualiza o pedido com o link/ID da preferência
                novoPedido.MercadoPagoPreferenceId = initPoint;
                await _pedidoRepository.UpdatePedidoAsync(novoPedido);

                // Limpa o carrinho do usuário
                await _carrinhoRepository.ClearCarrinhoAsync(user.Id);

                _logger.LogInformation($"Redirecionando para MP: {initPoint}");

                // Redireciona o usuário para a tela de pagamento
                return Redirect(initPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro Crítico no Mercado Pago para pedido {novoPedido.Id}");

                // Se falhou o pagamento, cancela o pedido para não ficar pendente eternamente
                if (novoPedido.Id > 0)
                {
                    novoPedido.Status = StatusPedidoEnum.Cancelado;
                    await _pedidoRepository.UpdatePedidoAsync(novoPedido);
                }

                TempData["ErroPagamento"] = "Ocorreu um erro ao conectar com o Mercado Pago. Tente novamente.";
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }
        }

        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            ViewData["OrderId"] = id;
            return View();
        }
    }
}