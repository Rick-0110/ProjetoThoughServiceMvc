using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Models.ModelCheckout;
using ToughService.Services;

namespace ToughService.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutViewModelBuilder _checkoutViewModelBuilder;

        public CheckoutController(ICheckoutViewModelBuilder checkoutViewModelBuilder)
        {
            _checkoutViewModelBuilder = checkoutViewModelBuilder;
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

            int novoOrderId = 987654;
            return RedirectToAction(nameof(Confirmation), new { id = novoOrderId });
        }

        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            ViewData["OrderId"] = id;
            return View();
        }
    }
}