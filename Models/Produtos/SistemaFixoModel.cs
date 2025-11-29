namespace ToughService.Models.Produtos
{
    public class SistemaFixoModel : ProdutoBaseModel
    {
        public string TipoSistema { get; set; }
        public string CapacidadeSistema { get; set; }
        public string AreaCobertura { get; set; }

        public SistemaFixoModel()
        {
            Categoria = CategoriaEnum.SistemasFixos;
        }
    }
}