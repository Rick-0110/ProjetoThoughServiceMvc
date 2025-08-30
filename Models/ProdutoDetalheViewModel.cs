using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;

namespace ToughService.Models
{
    public class ProdutoDetalheViewModel
    {
        public ProdutoModel Produto { get; set; }
       
        public List<ProdutoModel> OutrosProdutos { get; internal set; }
    }
}