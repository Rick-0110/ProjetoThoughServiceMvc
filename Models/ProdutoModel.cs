using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Models;

namespace ToughService.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        public int CategoriaId { get; set; }
        public CategoriaEnum? Categoria { get; set; }


        public string Sku { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; } 
        public decimal? Peso { get; set; } 
        public bool Ativo { get; set; }

        [NotMapped] 
        public IFormFile? Imagem { get; set; }

    }
}