using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.Produtos
{
    public class EPRModel : ProdutoBaseModel
    {
        [Required]
        public string TipoEPR { get; set; }

        public string TipoFiltro { get; set; }

        [Required]
        public string CertificacaoCA { get; set; }

        public string Tamanho { get; set; }
        public string Norma { get; set; }

        public EPRModel()
        {
            Categoria = CategoriaEnum.EPR;
        }
    }
}