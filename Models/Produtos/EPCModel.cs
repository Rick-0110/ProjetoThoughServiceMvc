using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.Produtos
{
    public class EPCModel : ProdutoBaseModel
    {
        [Required]
        public string TipoEPC { get; set; }

        public string AreaProtecao { get; set; }
        public string Dimensoes { get; set; }
        public string Material { get; set; }
        public string Norma { get; set; }

        public EPCModel()
        {
            Categoria = CategoriaEnum.EPC;
        }
    }
}