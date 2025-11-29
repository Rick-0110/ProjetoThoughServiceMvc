using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ToughService.Models;
using ToughService.Models.Produtos;

namespace ToughService.Data
{
    public class BancoContext : IdentityDbContext<ApplicationUser>
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        // ========================================================================
        // TABELA ÚNICA (TPH)
        // ========================================================================
        public DbSet<ProdutoBaseModel> Produtos { get; set; }

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
        public DbSet<ProdutoModel> ProdutosLegado { get; set; }

        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<ChamadoModel> Chamados { get; set; }
        public DbSet<ItemCarrinhoModel> ItensCarrinho { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CONFIGURAÇÃO TPH - Tabela Única
            modelBuilder.Entity<ProdutoBaseModel>()
                .ToTable("Produtos")
                .HasIndex(p => p.Sku).IsUnique();

            modelBuilder.Entity<ProdutoBaseModel>()
                .HasDiscriminator<string>("TipoProduto")
                .HasValue<ExtintorModel>("Extintor")
                .HasValue<MangueiraModel>("Mangueira")
                .HasValue<HidranteModel>("Hidrante")
                .HasValue<AcessorioModel>("Acessorio")
                .HasValue<EPIModel>("EPI")
                .HasValue<EPRModel>("EPR")
                .HasValue<EPCModel>("EPC")
                .HasValue<PortaCortaFogoModel>("PortaCortaFogo")
                .HasValue<SistemaFixoModel>("SistemaFixo")
                .HasValue<SistemaDeteccaoModel>("SistemaDeteccao")
                .HasValue<EquipamentoArMandadoModel>("ArMandado")
                .HasValue<ProdutoModel>("Generico");

            // Configurar relacionamento com ItensCarrinho
            modelBuilder.Entity<ItemCarrinhoModel>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relacionamento com PedidoItens
            modelBuilder.Entity<PedidoItemModel>()
                .HasOne(p => p.Produto)
                .WithMany()
                .HasForeignKey(p => p.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}