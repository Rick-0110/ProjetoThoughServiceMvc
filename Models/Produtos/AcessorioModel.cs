using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.Produtos
{
    public class AcessorioModel : ProdutoBaseModel
    {
        [Required]
        [StringLength(100)]
        public string TipoAcessorio { get; set; }

        [StringLength(100)]
        public string Dimensoes { get; set; }

        [StringLength(50)]
        public string Material { get; set; }

        [StringLength(50)]
        public string? Cor { get; set; }

        public AcessorioModel()
        {
            Categoria = CategoriaEnum.Acessorios;
        }
    }
}