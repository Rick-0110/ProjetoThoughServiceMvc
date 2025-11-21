namespace ToughService.Models.ModelCheckout
{
    public class CheckoutShippingOptionViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
    }
}

