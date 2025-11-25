using System.ComponentModel.DataAnnotations;
namespace ToughService.Models.LoginModels
{
    public class LoginModel
    {
           [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}