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

            if (!produtoAntigo.Categoria.HasValue)
                return null;

            return produtoAntigo.Categoria.Value switch
            {
                CategoriaEnum.Extintores => new ExtintorModel
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
                    Peso = produtoAntigo.Peso ?? 0,
                    DataRecarga = DateTime.Now, // Valor padrão, deve ser atualizado
                    TipoAgente = produtoAntigo.Sku_Agente,
                    Capacidade = produtoAntigo.Sku_Capacidade
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
                    Comprimento = ExtrairComprimento(produtoAntigo.Sku_Capacidade),
                    Diametro = 0, // Valor padrão
                    TipoMaterial = "Não especificado"
                },
                // Adicionar outros casos conforme necessário
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

