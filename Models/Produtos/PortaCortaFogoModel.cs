using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models.Produtos
{
    public class PortaCortaFogoModel : ProdutoBaseModel
    {
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Largura { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Altura { get; set; }

        public string Material { get; set; }
        public string TempoResistenciaFogo { get; set; }
        public string TipoAbertura { get; set; }
        public string Certificacao { get; set; }

        public PortaCortaFogoModel()
        {
            Categoria = CategoriaEnum.PortasCortaFogo;
        }
    }
}