using System;
using System.Linq;
using ToughService.Models;
using ToughService.Models.Produtos;

namespace ToughService.Models.Produtos
{
    /// <summary>
    /// Classe auxiliar para conversão entre ProdutoModel antigo e os novos Models específicos
    /// </summary>
    public static class ProdutoConverter
    {
        /// <summary>
        /// Converte um ProdutoModel antigo para o Model específico baseado na categoria
        /// </summary>
        public static IProdutoBase ConverterParaModelEspecifico(ProdutoModel produtoAntigo)
        {
            if (produtoAntigo == null)
                return null;

            // REMOVIDO: if (!produtoAntigo.Categoria.HasValue)
            // A categoria agora é obrigatória, então sempre existe.

            // REMOVIDO: .Value no switch
            return produtoAntigo.Categoria switch
            {
                CategoriaEnum.Extintores => new ExtintorModel
                {
                    // Propriedades Base (IProdutoBase)
                    Id = produtoAntigo.Id,
                    Nome = produtoAntigo.Nome,
                    Descricao = produtoAntigo.Descricao,
                    Preco = produtoAntigo.Preco,
                    ImagemUrl = produtoAntigo.ImagemUrl,
                    Sku = produtoAntigo.Sku,
                    Marca = produtoAntigo.Marca,
                    Quantidade = produtoAntigo.Quantidade,
                    Ativo = produtoAntigo.Ativo,

                    // Campos de SKU Genéricos
                    Sku_Tipo = produtoAntigo.Sku_Tipo,
                    Sku_Agente = produtoAntigo.Sku_Agente,
                    Sku_Capacidade = produtoAntigo.Sku_Capacidade,
                    Sku_Modelo = produtoAntigo.Sku_Modelo,

                    // Propriedades Específicas do ExtintorModel
                    // Mapeando o antigo 'Peso' (genérico) para 'PesoLiquido' (específico)
                    PesoLiquido = produtoAntigo.Peso ?? 0,

                    // Atenção: Certifique-se de que ExtintorModel tem estas propriedades:
                    DataFabricacao = DateTime.Now, // Placeholder
                                                   // DataRecarga = DateTime.Now, // Se existir no ExtintorModel

                    // Mapeia strings antigas para campos específicos se necessário
                    NormaReferencia = "NBR 15808" // Valor padrão ou extraído de algum lugar
                },

                CategoriaEnum.Mangueiras => new MangueiraModel
                {
                    Id = produtoAntigo.Id,
                    Nome = produtoAntigo.Nome,
                    Descricao = produtoAntigo.Descricao,
                    Preco = produtoAntigo.Preco,
                    ImagemUrl = produtoAntigo.ImagemUrl,
                    Sku = produtoAntigo.Sku,
                    Marca = produtoAntigo.Marca,
                    Quantidade = produtoAntigo.Quantidade,
                    Ativo = produtoAntigo.Ativo,

                    Sku_Tipo = produtoAntigo.Sku_Tipo,
                    Sku_Agente = produtoAntigo.Sku_Agente,
                    Sku_Capacidade = produtoAntigo.Sku_Capacidade,
                    Sku_Modelo = produtoAntigo.Sku_Modelo,

                    // Propriedades Específicas de MangueiraModel
                    Comprimento = ExtrairComprimento(produtoAntigo.Sku_Capacidade),
                    Diametro = 0, // Valor padrão
                    TipoMaterial = "Não especificado"
                },


                _ => null
            };
        }

        private static decimal ExtrairComprimento(string skuCapacidade)
        {
            // Tenta extrair o comprimento do formato "10MT" ou similar
            if (string.IsNullOrWhiteSpace(skuCapacidade))
                return 0;

            var numeros = new string(skuCapacidade.Where(char.IsDigit).ToArray());
            if (decimal.TryParse(numeros, out var comprimento))
                return comprimento;

            return 0;
        }
    }
}