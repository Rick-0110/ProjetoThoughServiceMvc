using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ToughService.Models;
using ToughService.Models.Produtos; // Importante para achar os novos models

namespace ToughService.Data
{
    public class BancoContext : IdentityDbContext<ApplicationUser>
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        // ========================================================================
        // TABELAS DE PRODUTOS ESPECÍFICAS POR CATEGORIA
        // ========================================================================
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

        // Tabela legada de produtos genéricos
        public DbSet<ProdutoModel> Produtos { get; set; }

        // Outros DbSets do sistema
        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<ChamadoModel> Chamados { get; set; }
        public DbSet<ItemCarrinhoModel> ItensCarrinho { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ====================================================================
            // CONFIGURAÇÃO DAS TABELAS DE PRODUTOS
            // ====================================================================

            // Configuração para Extintores
            modelBuilder.Entity<ExtintorModel>()
                .ToTable("Extintores");
            modelBuilder.Entity<ExtintorModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Mangueiras
            modelBuilder.Entity<MangueiraModel>()
                .ToTable("Mangueiras");
            modelBuilder.Entity<MangueiraModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Hidrantes
            modelBuilder.Entity<HidranteModel>()
                .ToTable("Hidrantes");
            modelBuilder.Entity<HidranteModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Acessórios
            modelBuilder.Entity<AcessorioModel>()
                .ToTable("Acessorios");
            modelBuilder.Entity<AcessorioModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para EPIs
            modelBuilder.Entity<EPIModel>()
                .ToTable("EPIs");
            modelBuilder.Entity<EPIModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para EPRs
            modelBuilder.Entity<EPRModel>()
                .ToTable("EPRs");
            modelBuilder.Entity<EPRModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para EPCs
            modelBuilder.Entity<EPCModel>()
                .ToTable("EPCs");
            modelBuilder.Entity<EPCModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Portas Corta-Fogo
            modelBuilder.Entity<PortaCortaFogoModel>()
                .ToTable("PortasCortaFogo");
            modelBuilder.Entity<PortaCortaFogoModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Sistemas Fixos
            modelBuilder.Entity<SistemaFixoModel>()
                .ToTable("SistemasFixos");
            modelBuilder.Entity<SistemaFixoModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Sistemas de Detecção
            modelBuilder.Entity<SistemaDeteccaoModel>()
                .ToTable("SistemasDeteccao");
            modelBuilder.Entity<SistemaDeteccaoModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Equipamentos Ar Mandado
            modelBuilder.Entity<EquipamentoArMandadoModel>()
                .ToTable("EquipamentosArMandado");
            modelBuilder.Entity<EquipamentoArMandadoModel>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Configuração para Produtos legados (tabela genérica)
            modelBuilder.Entity<ProdutoModel>()
                .ToTable("Produtos")
                .HasIndex(p => p.Sku)
                .IsUnique();
        }
    }
}