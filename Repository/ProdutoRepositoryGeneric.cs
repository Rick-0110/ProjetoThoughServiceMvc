using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Data;
using ToughService.Models.Produtos;

namespace ToughService.Repository
{
    /// <summary>
    /// Repositório genérico para trabalhar com os novos Models específicos por categoria
    /// </summary>
    public class ProdutoRepositoryGeneric : IProdutoRepositoryGeneric
    {
        private readonly BancoContext _context;

        public ProdutoRepositoryGeneric(BancoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, IProdutoBase
        {
            var dbSet = GetDbSet<T>();
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class, IProdutoBase
        {
            var dbSet = GetDbSet<T>();
            return await dbSet.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<T> AddAsync<T>(T produto) where T : class, IProdutoBase
        {
            var dbSet = GetDbSet<T>();
            await dbSet.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<bool> RemoveAsync<T>(int id) where T : class, IProdutoBase
        {
            var produto = await GetByIdAsync<T>(id);
            if (produto == null)
                return false;

            var dbSet = GetDbSet<T>();
            dbSet.Remove(produto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T> UpdateAsync<T>(T produto) where T : class, IProdutoBase
        {
            var dbSet = GetDbSet<T>();
            dbSet.Update(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<IEnumerable<IProdutoBase>> GetAllProdutosAsync()
        {
            var todosProdutos = new List<IProdutoBase>();

            // Busca produtos de todas as categorias e converte para IProdutoBase
            var extintores = await _context.Extintores.ToListAsync();
            todosProdutos.AddRange(extintores);

            var mangueiras = await _context.Mangueiras.ToListAsync();
            todosProdutos.AddRange(mangueiras);

            var hidrantes = await _context.Hidrantes.ToListAsync();
            todosProdutos.AddRange(hidrantes);

            var acessorios = await _context.Acessorios.ToListAsync();
            todosProdutos.AddRange(acessorios);

            var epis = await _context.EPIs.ToListAsync();
            todosProdutos.AddRange(epis);

            var eprs = await _context.EPRs.ToListAsync();
            todosProdutos.AddRange(eprs);

            var epcs = await _context.EPCs.ToListAsync();
            todosProdutos.AddRange(epcs);

            var portas = await _context.PortasCortaFogo.ToListAsync();
            todosProdutos.AddRange(portas);

            var sistemasFixos = await _context.SistemasFixos.ToListAsync();
            todosProdutos.AddRange(sistemasFixos);

            var sistemasDeteccao = await _context.SistemasDeteccao.ToListAsync();
            todosProdutos.AddRange(sistemasDeteccao);

            var equipamentosAr = await _context.EquipamentosArMandado.ToListAsync();
            todosProdutos.AddRange(equipamentosAr);

            return todosProdutos;
        }

        public async Task<string> VerificarSkuExistenteAsync(string sku)
        {
            var todosProdutos = await GetAllProdutosAsync();
            var produtoComSku = todosProdutos.FirstOrDefault(p => p.Sku == sku);
            return produtoComSku?.Sku;
        }

        private DbSet<T> GetDbSet<T>() where T : class
        {
            var tipo = typeof(T);
            
            if (tipo == typeof(ExtintorModel))
                return _context.Set<T>();
            if (tipo == typeof(MangueiraModel))
                return _context.Set<T>();
            if (tipo == typeof(HidranteModel))
                return _context.Set<T>();
            if (tipo == typeof(AcessorioModel))
                return _context.Set<T>();
            if (tipo == typeof(EPIModel))
                return _context.Set<T>();
            if (tipo == typeof(EPRModel))
                return _context.Set<T>();
            if (tipo == typeof(EPCModel))
                return _context.Set<T>();
            if (tipo == typeof(PortaCortaFogoModel))
                return _context.Set<T>();
            if (tipo == typeof(SistemaFixoModel))
                return _context.Set<T>();
            if (tipo == typeof(SistemaDeteccaoModel))
                return _context.Set<T>();
            if (tipo == typeof(EquipamentoArMandadoModel))
                return _context.Set<T>();

            throw new ArgumentException($"Tipo {typeof(T).Name} não é suportado.");
        }
    }
}

