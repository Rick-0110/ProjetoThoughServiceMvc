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
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            ICheckoutViewModelBuilder checkoutViewModelBuilder,
            IEmailService emailService,
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            UserManager<ApplicationUser> userManager)
        {
            _checkoutViewModelBuilder = checkoutViewModelBuilder;
            _emailService = emailService;
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _userManager = userManager;
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

            return View("~/Views/Carrinho/Checkout.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(CheckoutViewModel model)
        {
            var hydratedModel = await _checkoutViewModelBuilder.BuildAsync(model);

            if (hydratedModel.CartItems == null || !hydratedModel.CartItems.Any())
            {
                TempData["ErroCarrinho"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            var totalCalculado = hydratedModel.Total;
            if (Math.Abs(totalCalculado - model.Total) > 0.01m)
            {
                ModelState.AddModelError(string.Empty, "Erro de cálculo no total. O pedido não pode ser processado.");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            bool pagamentoBemSucedido = true;
            if (!pagamentoBemSucedido)
            {
                ModelState.AddModelError("PaymentError", "O pagamento falhou.");
                return View("~/Views/Carrinho/Checkout.cshtml", hydratedModel);
            }

            // ==================================================================
            // 4. SALVAR O PEDIDO NO BANCO 
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

            await _pedidoRepository.AddPedidoAsync(novoPedido);

            // ==================================================================
            // 5. ENVIAR E-MAIL COM TABELA DE PRODUTOS
            // ==================================================================
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"<h1>Obrigado, {novoPedido.NomeCliente}!</h1>");
                sb.AppendLine($"<p>Seu pedido <strong>#{novoPedido.Id}</strong> foi recebido com sucesso.</p>");

                sb.AppendLine("<h3>Resumo do Pedido:</h3>");
                sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse; width:100%; max-width:600px; font-family: Arial, sans-serif;'>");
                sb.AppendLine("<tr style='background-color:#f2f2f2; text-align:left;'><th>Produto</th><th>Qtd</th><th>Preço</th></tr>");

                foreach (var item in hydratedModel.CartItems)
                {
                    sb.AppendLine("<tr>");
                    // Assumindo que CheckoutCartItemViewModel tem ProductName, Quantity e Price
                    // Se os nomes forem diferentes lá também, me avise.
                    sb.AppendLine($"<td>{item.Nome}</td>");
                    sb.AppendLine($"<td style='text-align:center'>{item.Quantidade}</td>");
                    sb.AppendLine($"<td style='text-align:right'>R$ {item.PrecoUnitario:F2}</td>");
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");

                sb.AppendLine($"<p><strong>Subtotal:</strong> R$ {novoPedido.Subtotal:F2}</p>");
                sb.AppendLine($"<p><strong>Frete:</strong> R$ {novoPedido.CustoEnvio:F2}</p>");
                sb.AppendLine($"<h2 style='color:#28a745;'>Total: R$ {novoPedido.Total:F2}</h2>");

                sb.AppendLine("<hr>");
                sb.AppendLine($"<p><strong>Endereço de Entrega:</strong><br>{novoPedido.Logradouro}, {novoPedido.Numero}<br>{novoPedido.Cidade} - {novoPedido.Estado}<br>CEP: {novoPedido.Cep}</p>");
                sb.AppendLine("<p>Avisaremos quando o status mudar.</p>");
                sb.AppendLine("<p>Att,<br>Equipe Tough Service</p>");
                await _emailService.SendEmailAsync(novoPedido.EmailCliente, $"Pedido #{novoPedido.Id} Confirmado - Tough Service", sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO ENVIAR EMAIL: {ex.Message}");
            }

            await _carrinhoRepository.ClearCarrinhoAsync(user.Id);

            return RedirectToAction(nameof(Confirmation), new { id = novoPedido.Id });
        }

        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            ViewData["OrderId"] = id;
            return View();
        }
    }
}