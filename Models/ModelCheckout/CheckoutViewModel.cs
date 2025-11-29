using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.ModelCheckout
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Informe o seu nome completo.")]
        public string CheckoutName { get; set; } = string.Empty;

        public string CheckoutDocument { get; set; } = string.Empty;

        public string SelectedAddressId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe um e-mail válido.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string CheckoutEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe um telefone para contato.")]
        public string CheckoutPhone { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        /// <summary>
        /// Quantidade total de itens de extintor no carrinho (soma das quantidades de produtos cujo nome contenha "Extintor").
        /// </summary>
        public int TotalExtintores { get; set; }

        [Required(ErrorMessage = "Informe o CEP.")]
        public string CheckoutCep { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o estado.")]
        public string CheckoutEstado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a cidade.")]
        public string CheckoutCidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o endereço.")]
        public string CheckoutEndereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o número.")]
        public string CheckoutNumero { get; set; } = string.Empty;

        public string CheckoutComplemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um método de envio.")]
        public string ShippingMethod { get; set; } = string.Empty;

        public string CouponInput { get; set; } = string.Empty;
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Selecione a forma de pagamento.")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string CardNumber { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
        public string CardExpiration { get; set; } = string.Empty;
        public string CardCvv { get; set; } = string.Empty;
        public string Installments { get; set; } = "1";
        public bool SaveCard { get; set; }

        public List<CheckoutCartItemViewModel> CartItems { get; set; } = new();
        public List<CheckoutAddressViewModel> Addresses { get; set; } = new();
        public List<CheckoutShippingOptionViewModel> ShippingOptions { get; set; } = new();
        public List<CheckoutPaymentOptionViewModel> PaymentOptions { get; set; } = new();
    }
}