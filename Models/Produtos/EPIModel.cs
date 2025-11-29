using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.Produtos
{
    public class EPIModel : ProdutoBaseModel
    {
        [Required]
        public string TipoEPI { get; set; }

        [Required]
        public string CertificacaoCA { get; set; }

        public string Tamanho { get; set; }
        public string Material { get; set; }
        public string Norma { get; set; }

        public EPIModel()
        {
            Categoria = CategoriaEnum.EPI;
        }
    }
}