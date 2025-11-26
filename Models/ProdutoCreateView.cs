using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class ProdutoCreateViewModel
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    public string Nome { get; set; }

    // Propriedade para receber o UPLOAD DA IMAGEM do formulário.
    public IFormFile? Imagem { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    public string Descricao { get; set; }

    public string Sku { get; set; }
    public string Marca { get; set; }
    public int Quantidade { get; set; }
    public decimal? Peso { get; set; }
    public bool Ativo { get; set; }
}