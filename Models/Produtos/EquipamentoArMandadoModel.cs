namespace ToughService.Models.Produtos
{
    public class EquipamentoArMandadoModel : ProdutoBaseModel
    {
        public string TipoEquipamento { get; set; }
        public string Capacidade { get; set; }
        public string PressaoTrabalho { get; set; }
        public string Potencia { get; set; }
        public string Norma { get; set; }

        public EquipamentoArMandadoModel()
        {
            Categoria = CategoriaEnum.EquipamentosArMandado;
        }
    }
}