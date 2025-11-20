namespace ToughService.Models
{
    public class CheckoutAddressViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string? Complement { get; set; }
        public string CityState { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}

