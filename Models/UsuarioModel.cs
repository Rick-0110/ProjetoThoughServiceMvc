using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ToughService.Models
{
    public class UsuarioModel
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; }

    [Required]
    public string CpfCnpj { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Senha { get; set; }

    // 👇 Campo para indicar o tipo de perfil
    [Required]
    public string Perfil { get; set; } = "Cliente"; 
}

}