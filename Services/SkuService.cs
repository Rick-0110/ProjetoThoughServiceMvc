using System;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Models;
using ToughService.Repository;

namespace ToughService.Services
{
    public class SkuService : ISkuService
    {
        private readonly IProdutoRepository _produtoRepository;

        public SkuService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<string> GerarSkuAsync(ProdutoModel produto)
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
            var produtosExistentes = await _produtoRepository.GetAllProdutosAsync();
            var produtosComMesmoSkuBase = produtosExistentes
                .Where(p => !string.IsNullOrWhiteSpace(p.Sku) && p.Sku.StartsWith(skuBase))
                .ToList();

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
