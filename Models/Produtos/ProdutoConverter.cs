

namespace ToughService.Models.Produtos
{
    public static class ProdutoConverter
    {
        public static ProdutoBaseModel ConverterParaModelEspecifico(ProdutoModel produtoAntigo)
        {
            if (produtoAntigo == null) return null;

            ProdutoBaseModel produtoNovo = produtoAntigo.Categoria switch
            {
                CategoriaEnum.Extintores => new ExtintorModel
                {
                    PesoLiquido = produtoAntigo.Peso ?? 0,
                    DataRecarga = DateTime.Now,
                    TipoAgente = produtoAntigo.Sku_Agente ?? "Padrão",
                    Capacidade = produtoAntigo.Sku_Capacidade ?? "Padrão",
                    NormaReferencia = "NBR"
                },

                CategoriaEnum.Mangueiras => new MangueiraModel
                {
                    Comprimento = ExtrairNumero(produtoAntigo.Sku_Capacidade),
                    Diametro = 0,
                    TipoMaterial = "Padrão",
                    Peso = produtoAntigo.Peso
                },

                CategoriaEnum.Hidrantes => new HidranteModel
                {
                    TipoHidrante = "Padrão",
                    PressaoTrabalho = 0,
                    Rosca = "Storz",
                    Material = "Latão"
                },

                CategoriaEnum.Acessorios => new AcessorioModel
                {
                    TipoAcessorio = "Geral",
                    Dimensoes = "Padrão",
                    Material = "Diverso",
                    Cor = "Vermelho"
                },

                CategoriaEnum.EPI => new EPIModel
                {
                    TipoEPI = "Geral",
                    CertificacaoCA = "0000", // Obrigatório
                    Tamanho = "Único",
                    Material = "Padrão",
                    Norma = "NR6"
                },

                CategoriaEnum.EPR => new EPRModel
                {
                    TipoEPR = "Respirador",
                    TipoFiltro = "Mecânico",
                    CertificacaoCA = "0000",
                    Tamanho = "Único",
                    Norma = "N/A"
                },

                CategoriaEnum.EPC => new EPCModel
                {
                    TipoEPC = "Sinalização",
                    AreaProtecao = "N/A",
                    Dimensoes = "Padrão",
                    Material = "PVC",
                    Norma = "N/A"
                },

                CategoriaEnum.PortasCortaFogo => new PortaCortaFogoModel
                {
                    Largura = 0,
                    Altura = 0,
                    TempoResistenciaFogo = "P90",
                    Material = "Aço",
                    TipoAbertura = "Direita",
                    Certificacao = "NBR"
                },

                CategoriaEnum.SistemasFixos => new SistemaFixoModel
                {
                    TipoSistema = "Sprinkler",
                    CapacidadeSistema = "N/A",
                    AreaCobertura = "N/A"
                },

                CategoriaEnum.SistemasDeDeteccao => new SistemaDeteccaoModel
                {
                    TipoDetecao = "Fumaça",
                    TipoSensor = "Óptico",
                    AreaCobertura = "N/A",
                    TensaoAlimentacao = "24V",
                    Norma = "NBR"
                },

                CategoriaEnum.EquipamentosArMandado => new EquipamentoArMandadoModel
                {
                    TipoEquipamento = "Cilindro",
                    Capacidade = "N/A",
                    PressaoTrabalho = "High",
                    Potencia = "N/A",
                    Norma = "N/A"
                },

                // Caso padrão (nunca deve acontecer se o Enum estiver certo)
                _ => new ProdutoModel()
            };

            // Copia os dados comuns (Base) do antigo para o novo
            CopiarDadosBase(produtoAntigo, produtoNovo);

            return produtoNovo;
        }

        private static void CopiarDadosBase(ProdutoModel origem, ProdutoBaseModel destino)
        {
            destino.Id = origem.Id;
            destino.Nome = origem.Nome;
            destino.Descricao = origem.Descricao;
            destino.Preco = origem.Preco;
            destino.ImagemUrl = origem.ImagemUrl;
            destino.CategoriaId = origem.CategoriaId;
            destino.Categoria = origem.Categoria;
            destino.Sku = origem.Sku;
            destino.Sku_Tipo = origem.Sku_Tipo;
            destino.Sku_Agente = origem.Sku_Agente;
            destino.Sku_Capacidade = origem.Sku_Capacidade;
            destino.Sku_Modelo = origem.Sku_Modelo;
            destino.Marca = origem.Marca;
            destino.Quantidade = origem.Quantidade;
            destino.Ativo = origem.Ativo;
            // Imagem (IFormFile) não precisa copiar pois já foi salva ou tratada antes
        }

        private static decimal ExtrairNumero(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0;
            var numeros = new string(texto.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
            if (decimal.TryParse(numeros.Replace(".", ","), out var resultado)) return resultado;
            return 0;
        }
    }
}