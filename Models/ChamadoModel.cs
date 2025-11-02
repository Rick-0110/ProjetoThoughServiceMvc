using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Models;

namespace ToughService.Models
{
    public class ChamadoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        public string NomeCliente { get; set; }

        public DateTime DataSolicitacao { get; set; }

        [Required(ErrorMessage = "A data desejada é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataDesejada { get; set; }

        [Required(ErrorMessage = "O tipo de serviço é obrigatório.")]
        public string TipoServico { get; set; } 

        [Required(ErrorMessage = "O tipo de equipamento é obrigatório.")]
        public string TipoExtintor { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "O logradouro é obrigatório.")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "O número é obrigatório.")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O estado (UF) é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "UF deve ter 2 caracteres.")]
        public string Estado { get; set; }

        public string? Complemento { get; set; } 

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Formato de telefone inválido.")]
        public string Telefone { get; set; }

        public string? Observacoes { get; set; } 

        public StatusChamadoEnum Status { get; set; }


       
        public string? UserId { get; set; }
       

        [ForeignKey("UserId")]
        [ValidateNever]
        public virtual ApplicationUser? User { get; set; }

    }
}