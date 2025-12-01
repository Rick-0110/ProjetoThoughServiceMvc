using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.LoginModels
{
    public class RedefinirSenhaModel
    {
        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Nova Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarNovaSenha { get; set; }

        public string Code { get; set; }
    }
}