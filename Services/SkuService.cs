using System;
using System.Threading.Tasks;
using ToughService.Models.Produtos;
using ToughService.Repository;

namespace ToughService.Services
{
    public class SkuService : ISkuService
    {
        // Usa o repositório genérico para verificar se o SKU já existe
        private readonly IProdutoRepositoryGeneric _produtoRepository;

        public SkuService(IProdutoRepositoryGeneric produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<string> GerarSkuAsync(ProdutoBaseModel produto)
        {
            // Gera o código base: TIPO-AGENTE-CAPACIDADE-MODELO
            // Ex: EXT-ABC-4KG-STD

            string prefixo = produto.Sku_Tipo?.ToUpper() ?? "GEN";
            string agente = produto.Sku_Agente?.ToUpper() ?? "XX";
            string capacidade = produto.Sku_Capacidade?.ToUpper() ?? "00";
            string modelo = produto.Sku_Modelo?.ToUpper() ?? "STD";

            string skuBase = $"{prefixo}-{agente}-{capacidade}-{modelo}";

            // Verifica duplicidade no banco
            string skuExistente = await _produtoRepository.VerificarSkuExistenteAsync(skuBase);

            if (skuExistente != null)
            {
                // Se já existe, adiciona 4 caracteres aleatórios no final para tornar único
                return $"{skuBase}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            }

            return skuBase;
        }
    }
}