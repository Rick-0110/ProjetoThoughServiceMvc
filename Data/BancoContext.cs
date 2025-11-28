using Microsoft.EntityFrameworkCore;
using ToughService.Models;
using ToughService.Models.Produtos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace ToughService.Data
{

    public class BancoContext : IdentityDbContext<ApplicationUser>
    {
        
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        public DbSet<ProdutoModel> Produtos { get; set; }

        // Novos Models específicos por categoria
        public DbSet<ExtintorModel> Extintores { get; set; }
        public DbSet<MangueiraModel> Mangueiras { get; set; }
        public DbSet<HidranteModel> Hidrantes { get; set; }
        public DbSet<AcessorioModel> Acessorios { get; set; }
        public DbSet<EPIModel> EPIs { get; set; }
        public DbSet<EPRModel> EPRs { get; set; }
        public DbSet<EPCModel> EPCs { get; set; }
        public DbSet<PortaCortaFogoModel> PortasCortaFogo { get; set; }
        public DbSet<SistemaFixoModel> SistemasFixos { get; set; }
        public DbSet<SistemaDeteccaoModel> SistemasDeteccao { get; set; }
        public DbSet<EquipamentoArMandadoModel> EquipamentosArMandado { get; set; }

        public DbSet<ApplicationUser> Usuarios { get; set; }

        public DbSet<ChamadoModel> Chamados { get; set; }

        public DbSet<ItemCarrinhoModel> ItensCarrinho { get; set; }

        public DbSet<PedidoModel> Pedidos { get; set; }

    }

}