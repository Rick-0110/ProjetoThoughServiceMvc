using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToughService.Extensions;
using ToughService.Models;
using ToughService.Models.ModelCheckout;
using ToughService.Models.Produtos;
using ToughService.Repository;

namespace ToughService.Services
{
    public class CheckoutViewModelBuilder : ICheckoutViewModelBuilder
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutViewModelBuilder(
            ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CheckoutViewModel> BuildAsync(CheckoutViewModel? model = null)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Contexto HTTP indisponível.");
            var viewModel = model ?? new CheckoutViewModel();

            var cartItems = await LoadCartItemsAsync(httpContext.User);

            // Se o usuário veio do carrinho com itens selecionados, filtramos aqui
            var selectedIds = httpContext.Session?.GetObject<List<int>>("SelectedProdutoIds");
            if (selectedIds != null && selectedIds.Any())
            {
                cartItems = cartItems.Where(i => selectedIds.Contains(i.ProdutoId)).ToList();
                // limpa seleção após o uso
                httpContext.Session.Remove("SelectedProdutoIds");
            }

            viewModel.CartItems = cartItems;
            viewModel.Subtotal = viewModel.CartItems.Sum(i => i.Total);

            // Regra de qualificação: quantidade total de itens no carrinho (cada unidade conta)
            // Se quiser restringir apenas a extintores depois, podemos refinar aqui.
            viewModel.TotalExtintores = viewModel.CartItems.Sum(i => i.Quantidade);

            viewModel.PaymentOptions = BuildPaymentOptions();
            if (string.IsNullOrWhiteSpace(viewModel.PaymentMethod))
            {
                viewModel.PaymentMethod = viewModel.PaymentOptions.FirstOrDefault()?.Value ?? string.Empty;
            }

            viewModel.ShippingOptions = BuildShippingOptions();
            if (string.IsNullOrWhiteSpace(viewModel.ShippingMethod))
            {
                viewModel.ShippingMethod = viewModel.ShippingOptions.FirstOrDefault(o => o.IsDefault)?.Value
                    ?? viewModel.ShippingOptions.FirstOrDefault()?.Value
                    ?? string.Empty;
            }

            viewModel.ShippingCost = viewModel.ShippingOptions
                .FirstOrDefault(o => o.Value.Equals(viewModel.ShippingMethod, StringComparison.OrdinalIgnoreCase))
                ?.Price ?? 0m;

            viewModel.Discount = CalculatePreviewDiscount(viewModel.CouponInput, viewModel.Subtotal, viewModel.ShippingCost);
            viewModel.Total = viewModel.Subtotal + viewModel.ShippingCost - viewModel.Discount;

            await PopulateBuyerDataAsync(viewModel, httpContext.User);

            viewModel.Addresses = BuildAddresses(viewModel);
            if (string.IsNullOrWhiteSpace(viewModel.SelectedAddressId) && viewModel.Addresses.Any())
            {
                viewModel.SelectedAddressId = viewModel.Addresses.First().Id;
            }

            return viewModel;
        }

        private async Task<List<CheckoutCartItemViewModel>> LoadCartItemsAsync(ClaimsPrincipal user)
        {
            if (user?.Identity == null)
            {
                return new List<CheckoutCartItemViewModel>();
            }

            if (user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new List<CheckoutCartItemViewModel>();
                }

                var carrinhoDb = await _carrinhoRepository.GetCarrinhoByUserIdAsync(userId);
                return carrinhoDb.Select(MapToCheckoutItem).ToList();
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null)
            {
                return new List<CheckoutCartItemViewModel>();
            }

            var carrinhoSessao = httpContext.Session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();
            var result = new List<CheckoutCartItemViewModel>();

            foreach (var item in carrinhoSessao)
            {
                if (item.Produto == null)
                {
                    item.Produto = await _produtoRepository.GetProdutoByIdAsync(item.ProdutoId);
                }

                result.Add(MapToCheckoutItem(item));
            }

            return result;
        }

        private static CheckoutCartItemViewModel MapToCheckoutItem(ItemCarrinhoModel item)
        {
            var unitPrice = item.Produto?.Preco ?? item.PrecoUnitario;

            var produtoBase = item.Produto as ProdutoBaseModel;
            var nomeProduto = item.Produto?.Nome ?? string.Empty;

            // Considera extintor se:
            // - a categoria do produto é Extintores OU
            // - o nome do produto contém "Extintor" (case-insensitive)
            var isExtintor =
                (produtoBase != null && produtoBase.Categoria == CategoriaEnum.Extintores) ||
                (!string.IsNullOrWhiteSpace(nomeProduto) &&
                 nomeProduto.IndexOf("Extintor", StringComparison.OrdinalIgnoreCase) >= 0);

            return new CheckoutCartItemViewModel
            {
                ProdutoId = item.ProdutoId,
                Nome = item.Produto?.Nome ?? "Produto",
                Sku = item.Produto?.Sku,
                Quantidade = item.Quantidade,
                PrecoUnitario = unitPrice,
                ImagemUrl = item.Produto?.ImagemUrl,
                IsExtintor = isExtintor
            };
        }

        private List<CheckoutShippingOptionViewModel> BuildShippingOptions()
        {
            return new List<CheckoutShippingOptionViewModel>
            {
                new()
                {
                    Value = "delivery",
                    Title = "Entrega na sua empresa/casa",
                    Description = "Receba no endereço escolhido sem custo adicional",
                    Price = 0m,
                    IsDefault = true
                },
                new()
                {
                    Value = "pickup",
                    Title = "Retirar na Tough Service",
                    Description = "Disponível para retirada em até 2h na empresa",
                    Price = 0m
                }
            };
        }

        private List<CheckoutPaymentOptionViewModel> BuildPaymentOptions()
        {
            return new List<CheckoutPaymentOptionViewModel>
            {
                new()
                {
                    Value = "card",
                    IconCss = "fas fa-credit-card",
                    Label = "Cartão",
                    Description = "Pague com cartão de crédito"
                },
                new()
                {
                    Value = "pix",
                    IconCss = "fas fa-qrcode",
                    Label = "Pix",
                    Description = "Pagamento instantâneo"
                },
                new()
                {
                    Value = "boleto",
                    IconCss = "fas fa-barcode",
                    Label = "Boleto",
                    Description = "Pague em até 3 dias úteis"
                }
            };
        }

        private List<CheckoutAddressViewModel> BuildAddresses(CheckoutViewModel viewModel)
        {
            var addresses = new List<CheckoutAddressViewModel>();

            if (!string.IsNullOrWhiteSpace(viewModel.CheckoutEndereco) &&
                !string.IsNullOrWhiteSpace(viewModel.CheckoutCidade) &&
                !string.IsNullOrWhiteSpace(viewModel.CheckoutEstado) &&
                !string.IsNullOrWhiteSpace(viewModel.CheckoutCep))
            {
                addresses.Add(new CheckoutAddressViewModel
                {
                    Id = "current",
                    Label = "Endereço informado",
                    Street = $"{viewModel.CheckoutEndereco}, {viewModel.CheckoutNumero}",
                    Complement = viewModel.CheckoutComplemento,
                    CityState = $"{viewModel.CheckoutCidade}/{viewModel.CheckoutEstado}",
                    Cep = viewModel.CheckoutCep,
                    IsPrimary = true
                });
            }

            return addresses;
        }

        private async Task PopulateBuyerDataAsync(CheckoutViewModel model, ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var applicationUser = await _userManager.GetUserAsync(user);
            if (applicationUser == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(model.CheckoutName))
            {
                model.CheckoutName = applicationUser.Nome ?? applicationUser.UserName ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(model.CheckoutDocument) && !string.IsNullOrWhiteSpace(applicationUser.CpfCnpj))
            {
                model.CheckoutDocument = applicationUser.CpfCnpj;
            }

            if (string.IsNullOrWhiteSpace(model.CheckoutEmail))
            {
                model.CheckoutEmail = applicationUser.Email ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(model.CheckoutPhone))
            {
                model.CheckoutPhone = applicationUser.PhoneNumber ?? string.Empty;
            }
        }

        private static decimal CalculatePreviewDiscount(string couponInput, decimal subtotal, decimal shippingCost)
        {
            if (string.IsNullOrWhiteSpace(couponInput))
            {
                return 0m;
            }

            var code = couponInput.Trim().ToUpperInvariant();
            return code switch
            {
                "TGS10" => Math.Round(subtotal * 0.10m, 2),
                "FRETEGRATIS" => Math.Min(shippingCost, 12.90m),
                _ => 0m
            };
        }
    }
}

