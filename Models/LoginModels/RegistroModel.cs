using System.ComponentModel.DataAnnotations;

namespace ToughService.Models
{
    public class RegistroModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF ou CNPJ é obrigatório.")]
        public string CpfCnpj { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        [DataType(DataType.Password)]
        public string ConfirmaSenha { get; set; }
    }
}
