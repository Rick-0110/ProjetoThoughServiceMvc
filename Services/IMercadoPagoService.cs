using ToughService.Models;
using ToughService.Models.ModelCheckout;

namespace ToughService.Services
{
    public interface IMercadoPagoService
    {
        Task<string> CreatePreferenceAsync(CheckoutViewModel checkout, int pedidoId, string userId);
        Task<bool> ProcessPaymentNotificationAsync(string paymentId);
        Task<MercadoPagoPaymentStatus> GetPaymentStatusAsync(string paymentId);
    }

    public class MercadoPagoPaymentStatus
    {
        public string Status { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}

