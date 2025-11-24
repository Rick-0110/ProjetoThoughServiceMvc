
using System.ComponentModel.DataAnnotations;

namespace ToughService.Models
{
    public class ExternalLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Nome { get; set; }
    }
}