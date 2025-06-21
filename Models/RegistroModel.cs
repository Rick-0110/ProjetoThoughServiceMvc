using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models
{
    public class RegistroModel
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        [RegularExpression(@"\d{11}", ErrorMessage = "CPF deve ter 11 dígitos.")]
        public string CpfCnpj { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmaSenha { get; set; }
    }
}