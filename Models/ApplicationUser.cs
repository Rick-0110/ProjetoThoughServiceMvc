using Microsoft.AspNetCore.Identity;
using ToughService.Models;

namespace ToughService.Models
{
    public class ApplicationUser : IdentityUser
    {
      
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }

        public string? ProfilePicturePath { get; set; }

    }
}