using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughService.Models;
using ProjetoThoughServiceMvc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace ToughService.Data
{

    public class BancoContext : IdentityDbContext<ApplicationUser>
    {
        
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }



        // tabela de produtos no banco de dados
        public DbSet<ProdutoModel> Produtos { get; set; }

          public DbSet<ApplicationUser> Usuarios { get; set; }
    }

}