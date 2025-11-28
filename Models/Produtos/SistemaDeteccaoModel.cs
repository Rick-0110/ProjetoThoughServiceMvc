namespace ToughService.Models.Produtos
{
    public class SistemaDeteccaoModel : ProdutoBaseModel
    {
        public string TipoDetecao { get; set; }
        public string AreaCobertura { get; set; }
        public string TipoSensor { get; set; }
        public string TensaoAlimentacao { get; set; }
        public string Norma { get; set; }

        public SistemaDeteccaoModel()
        {
            Categoria = CategoriaEnum.SistemasDeDeteccao;
        }
    }
}