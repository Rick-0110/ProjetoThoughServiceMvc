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

    [Required(ErrorMessage = "O Tipo/Prefix é obrigatório (Ex: EAT).")]
    public string Sku_Tipo { get; set; } // Ex: EAT (Extintor Automático) / MGA (Mangueira)

    [Required(ErrorMessage = "O Agente Extintor é obrigatório (Ex: PO / CO2).")]
    public string Sku_Agente { get; set; } // Ex: PO (Pó Químico) / AGUA

    [Required(ErrorMessage = "A Capacidade é obrigatória (Ex: 06KG / 10MT).")]
    public string Sku_Capacidade { get; set; } // Ex: 06KG (Capacidade) / 10MT (Comprimento da Mangueira)

    [Required(ErrorMessage = "O Modelo é obrigatório (Ex: MOD2).")]
    public string Sku_Modelo { get; set; } // Identificador de modelo/fabricante interno
}