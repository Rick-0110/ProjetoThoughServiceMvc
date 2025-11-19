namespace ToughService.Dtos
{
    public class CarrinhoItemResponseDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal => Preco * Quantidade;
    }
}
