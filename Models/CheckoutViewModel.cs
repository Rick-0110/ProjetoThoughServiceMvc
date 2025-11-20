using System.ComponentModel.DataAnnotations;
namespace ToughService.Models
{
    public class CheckoutViewModel
    {
        public string CheckoutDocument { get; set; }

        // 1. Dados de Endereço/Entrega
        // Se for um endereço existente:
        public string SelectedAddressId { get; set; }

        public string CheckoutCep { get; set; }
        public string CheckoutEstado { get; set; }
        public string CheckoutCidade { get; set; }
        public string CheckoutEndereco { get; set; }
        public string CheckoutNumero { get; set; }
        public string CheckoutComplemento { get; set; }

        public string ShippingMethod { get; set; } // Tipo de frete (express, standard, pickup)

        public string CouponInput { get; set; }
        public decimal Total { get; set; } 

        public string PaymentMethod { get; set; } 

        // Detalhes do Cartão (recebidos diretamente do formulário)
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardExpiration { get; set; }
        public string CardCvv { get; set; }
        public string Installments { get; set; }
        public bool SaveCard { get; set; }

    }
}