using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjetoThoughServiceMvc.Models;
using ToughService.Models;

namespace ProjetoThoughServiceMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
      
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }
      
    }
}