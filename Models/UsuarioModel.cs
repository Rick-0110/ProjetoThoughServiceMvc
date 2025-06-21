using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ToughService.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string CpfCnpj { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string Cep { get; set; } = string.Empty;
    public string Endereco { get; set; }= string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; }= string.Empty;
    }
}