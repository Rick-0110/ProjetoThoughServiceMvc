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

        public CheckoutController(
            ICheckoutViewModelBuilder checkoutViewModelBuilder,
            IEmailService emailService,
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            IMercadoPagoService mercadoPagoService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ILogger<CheckoutController> logger)
        {
            _checkoutViewModelBuilder = checkoutViewModelBuilder;
            _emailService = emailService;
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _mercadoPagoService = mercadoPagoService;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
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

            // Passar a PublicKey do Mercado Pago para a view
            ViewData["MercadoPagoPublicKey"] = _configuration["MercadoPagoSettings:PublicKey"] ?? string.Empty;

            return View("~/Views/Carrinho/Checkout.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(CheckoutViewModel model)
        {
            _logger.LogInformation($"ConfirmOrder chamado. PaymentMethod: {model.PaymentMethod}");
            
            var hydratedModel = await _checkoutViewModelBuilder.BuildAsync(model);

            if (hydratedModel.CartItems == null || !hydratedModel.CartItems.Any())
            {
                TempData["ErroCarrinho"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            // Aplicar valores do model ao hydratedModel
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
                _logger.LogWarning($"ModelState inválido. Erros: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            var totalCalculado = hydratedModel.Total;
            if (Math.Abs(totalCalculado - model.Total) > 0.01m)
            {
                ModelState.AddModelError(string.Empty, "Erro de cálculo no total. O pedido não pode ser processado.");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            // ==================================================================
            // 3. SALVAR O PEDIDO NO BANCO (ANTES DO PAGAMENTO)
            // ==================================================================
            var user = await _userManager.GetUserAsync(User);

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

            // Adicionar itens do pedido
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

            await _pedidoRepository.AddPedidoAsync(novoPedido);

            // ==================================================================
            // 4. PROCESSAR PAGAMENTO COM MERCADO PAGO
            // ==================================================================
            try
            {
                _logger.LogInformation($"Iniciando criação de preferência do Mercado Pago para pedido {novoPedido.Id}");
                
                // Criar preferência de pagamento no Mercado Pago
                var initPoint = await _mercadoPagoService.CreatePreferenceAsync(hydratedModel, novoPedido.Id, user.Id);

                if (string.IsNullOrEmpty(initPoint))
                {
                    _logger.LogError($"Erro ao criar preferência do Mercado Pago para pedido {novoPedido.Id} - InitPoint vazio");
                    ModelState.AddModelError("PaymentError", "Erro ao processar pagamento. Tente novamente.");
                    return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
                }

                _logger.LogInformation($"Preferência criada com sucesso. InitPoint: {initPoint}");

                // Salvar ID da preferência no pedido
                novoPedido.MercadoPagoPreferenceId = initPoint;
                await _pedidoRepository.UpdatePedidoAsync(novoPedido);

                _logger.LogInformation($"Redirecionando para o checkout do Mercado Pago: {initPoint}");
                
                // Redirecionar para o checkout do Mercado Pago
                return Redirect(initPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar preferência do Mercado Pago para pedido {novoPedido.Id}. Detalhes: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                
                // Se o pedido foi criado, marcar como erro
                if (novoPedido.Id > 0)
                {
                    novoPedido.Status = StatusPedidoEnum.Cancelado;
                    await _pedidoRepository.UpdatePedidoAsync(novoPedido);
                }
                
                TempData["ErroPagamento"] = $"Erro ao processar pagamento: {ex.Message}. Verifique as configurações do Mercado Pago.";
                ModelState.AddModelError("PaymentError", $"Erro ao processar pagamento. Tente novamente.");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            // O redirecionamento para o Mercado Pago já foi feito acima
            // O carrinho será limpo após a confirmação do pagamento (no webhook ou no retorno)
        }

        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            ViewData["OrderId"] = id;
            return View();
        }
    }
}