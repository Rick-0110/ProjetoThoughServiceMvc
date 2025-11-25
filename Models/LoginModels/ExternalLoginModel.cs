using System.ComponentModel.DataAnnotations;

namespace ToughService.Models.LoginModels
{

    public class ExternalLoginModel
    {
        [Required(ErrorMessage = "O campo Nome � obrigat�rio.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email � obrigat�rio.")]
        [EmailAddress(ErrorMessage = "E-mail inv�lido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }


    }
}