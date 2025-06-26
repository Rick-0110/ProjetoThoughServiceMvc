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
    }
}