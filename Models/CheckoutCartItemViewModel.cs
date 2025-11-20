namespace ToughService.Models
{
    public class CheckoutCartItemViewModel
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sku { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string? ImagemUrl { get; set; }

        public decimal Total => PrecoUnitario * Quantidade;
    }
}

