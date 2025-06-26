using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughService.Models;
using ProjetoThoughServiceMvc.Models;

namespace ToughService.Data
{
// Contexto do banco de dados que representa a conexão e as tabelas do sistema
public class BancoContext : DbContext
{
    // Construtor que recebe as opções de configuração do DbContext (como string de conexão)
    public BancoContext(DbContextOptions<BancoContext> options) : base(options)
    {
    }

    // Representa a tabela de usuários no banco de dados
    public DbSet<UsuarioModel> Usuarios { get; set; }

    // Representa a tabela de produtos no banco de dados
    public DbSet<ProdutoModel> Produtos { get; set; }
}

}