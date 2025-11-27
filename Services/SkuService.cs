using System;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Models;
using ToughService.Models.Produtos;
using ToughService.Repository;

namespace ToughService.Services
{
    public class SkuService : ISkuService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoRepositoryGeneric _produtoRepositoryGeneric;

        public SkuService(
            IProdutoRepository produtoRepository,
            IProdutoRepositoryGeneric produtoRepositoryGeneric)
        {
            _produtoRepository = produtoRepository;
            _produtoRepositoryGeneric = produtoRepositoryGeneric;
        }

        public async Task<string> GerarSkuAsync(IProdutoBase produto)
        {
            // Valida se os campos necessários estão preenchidos
            if (string.IsNullOrWhiteSpace(produto.Sku_Tipo) ||
                string.IsNullOrWhiteSpace(produto.Sku_Agente) ||
                string.IsNullOrWhiteSpace(produto.Sku_Capacidade) ||
                string.IsNullOrWhiteSpace(produto.Sku_Modelo))
            {
                throw new ArgumentException("Os campos Sku_Tipo, Sku_Agente, Sku_Capacidade e Sku_Modelo são obrigatórios para gerar o SKU.");
            }

            // Gera o SKU base baseado nos componentes
            string skuBase = $"{produto.Sku_Tipo}-{produto.Sku_Agente}-{produto.Sku_Capacidade}-{produto.Sku_Modelo}";

            // Verifica se já existe um produto com este SKU base
            // Busca em todos os produtos (antigos e novos) através dos repositórios
            var produtosAntigos = await _produtoRepository.GetAllProdutosAsync();
            var produtosNovos = await _produtoRepositoryGeneric.GetAllProdutosAsync();
            
            // Combina produtos antigos e novos para verificação de SKU
            var todosProdutos = produtosAntigos
                .Select(p => new { Sku = p.Sku })
                .Concat(produtosNovos.Select(p => new { Sku = p.Sku }))
                .Where(p => !string.IsNullOrWhiteSpace(p.Sku) && p.Sku.StartsWith(skuBase))
                .ToList();
            
            var produtosComMesmoSkuBase = todosProdutos;

            // Se não existe nenhum produto com este SKU base, retorna o SKU base
            if (!produtosComMesmoSkuBase.Any())
            {
                return skuBase;
            }

            // Se já existe, adiciona um sufixo numérico sequencial
            int sufixo = 1;
            string skuGerado = $"{skuBase}-{sufixo:D3}";

            // Enquanto o SKU gerado já existir, incrementa o sufixo
            while (produtosComMesmoSkuBase.Any(p => p.Sku.Equals(skuGerado, StringComparison.OrdinalIgnoreCase)))
            {
                sufixo++;
                skuGerado = $"{skuBase}-{sufixo:D3}";
            }

            return skuGerado;
        }
    }
}
