using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoThoughServiceMvc.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;
           public string Nome { get; set; }
        public string Categoria { get; set; }
    }
}