# 🔄 Refatoração da Modelagem de Produtos

## 📋 Resumo da Refatoração

Esta refatoração separa o `ProdutoModel` genérico em **Models específicos por categoria**, garantindo integridade de dados e geração correta de SKUs.

## 🏗️ Estrutura Criada

### 1. Interface Base
- **`IProdutoBase`** - Interface comum para todos os produtos
  - Campos comuns: Id, Nome, Descricao, Preco, ImagemUrl, Sku, Marca, Quantidade, Ativo
  - Campos SKU: Sku_Tipo, Sku_Agente, Sku_Capacidade, Sku_Modelo
  - Propriedade Categoria (readonly)

### 2. Models Específicos por Categoria

#### ✅ ExtintorModel
- **Campos específicos:**
  - `Peso` (decimal, obrigatório)
  - `DataRecarga` (DateTime, obrigatório)
  - `DataVencimento` (DateTime?, opcional)
  - `TipoAgente` (string)
  - `Capacidade` (string)

#### ✅ MangueiraModel
- **Campos específicos:**
  - `Comprimento` (decimal, obrigatório) - em metros
  - `Diametro` (decimal, obrigatório) - em polegadas ou mm
  - `TipoMaterial` (string)
  - `PressaoMaxima` (int?, opcional) - em PSI ou Bar

#### ✅ HidranteModel
- **Campos específicos:**
  - `TipoHidrante` (string) - Coluna, Parede, Subterrâneo
  - `PressaoTrabalho` (decimal, obrigatório)
  - `Rosca` (string)
  - `Material` (string)

#### ✅ AcessorioModel
- **Campos específicos:**
  - `TipoAcessorio` (string) - Abrigo, Placa, Suporte
  - `Dimensoes` (string)
  - `Material` (string)
  - `Cor` (string?, opcional)

#### ✅ EPIModel
- **Campos específicos:**
  - `TipoEPI` (string)
  - `CertificacaoCA` (string, obrigatório)
  - `Tamanho` (string?, opcional)
  - `Material` (string?, opcional)
  - `Norma` (string?, opcional)

#### ✅ EPRModel
- **Campos específicos:**
  - `TipoEPR` (string)
  - `TipoFiltro` (string, obrigatório) - P1, P2, P3
  - `CertificacaoCA` (string, obrigatório)
  - `Tamanho` (string?, opcional)
  - `Norma` (string?, opcional)

#### ✅ EPCModel
- **Campos específicos:**
  - `TipoEPC` (string)
  - `AreaProtecao` (string?, opcional)
  - `Dimensoes` (string?, opcional)
  - `Material` (string?, opcional)
  - `Norma` (string?, opcional)

#### ✅ PortaCortaFogoModel
- **Campos específicos:**
  - `Largura` (decimal, obrigatório)
  - `Altura` (decimal, obrigatório)
  - `Material` (string, obrigatório)
  - `TempoResistenciaFogo` (string, obrigatório) - 30min, 60min, 90min, 120min
  - `TipoAbertura` (string?, opcional)
  - `Certificacao` (string?, opcional)

#### ✅ SistemaFixoModel
- **Campos específicos:**
  - `TipoSistema` (string) - Sprinkler, Hidrante, Espuma
  - `CapacidadeSistema` (string?, opcional)
  - `AreaCobertura` (string?, opcional)
  - `PressaoTrabalho` (string?, opcional)
  - `Norma` (string?, opcional)

#### ✅ SistemaDeteccaoModel
- **Campos específicos:**
  - `TipoDetecao` (string) - Fumaça, Calor, Gás
  - `AreaCobertura` (string?, opcional)
  - `TipoSensor` (string?, opcional) - Óptico, Ionização, Térmico
  - `TensaoAlimentacao` (string?, opcional)
  - `Norma` (string?, opcional)

#### ✅ EquipamentoArMandadoModel
- **Campos específicos:**
  - `TipoEquipamento` (string) - Compressor, Reservatório
  - `Capacidade` (string, obrigatório)
  - `PressaoTrabalho` (string?, opcional)
  - `Potencia` (string?, opcional)
  - `Norma` (string?, opcional)

### 3. ViewModels de Criação

Cada categoria possui seu próprio ViewModel:
- `ExtintorCreateViewModel`
- `MangueiraCreateViewModel`
- `HidranteCreateViewModel`
- `AcessorioCreateViewModel`
- `EPICreateViewModel`
- `EPRCreateViewModel`
- `EPCCreateViewModel`
- `PortaCortaFogoCreateViewModel`
- `SistemaFixoCreateViewModel`
- `SistemaDeteccaoCreateViewModel`
- `EquipamentoArMandadoCreateViewModel`

## 🔧 Status da Refatoração

### ✅ Concluído

1. **Interface Base (`IProdutoBase`)**
   - ✅ Criada com todos os campos comuns
   - ✅ Propriedade Categoria readonly

2. **Models Específicos por Categoria**
   - ✅ ExtintorModel (com Peso, DataRecarga, DataVencimento)
   - ✅ MangueiraModel (com Comprimento, Diâmetro, TipoMaterial)
   - ✅ HidranteModel (com TipoHidrante, PressaoTrabalho)
   - ✅ AcessorioModel (com TipoAcessorio, Dimensoes)
   - ✅ EPIModel (com TipoEPI, CertificacaoCA)
   - ✅ EPRModel (com TipoEPR, TipoFiltro, CertificacaoCA)
   - ✅ EPCModel (com TipoEPC, AreaProtecao)
   - ✅ PortaCortaFogoModel (com Largura, Altura, TempoResistenciaFogo)
   - ✅ SistemaFixoModel (com TipoSistema, CapacidadeSistema)
   - ✅ SistemaDeteccaoModel (com TipoDetecao, TipoSensor)
   - ✅ EquipamentoArMandadoModel (com TipoEquipamento, Capacidade)

3. **ViewModels de Criação**
   - ✅ Todos os 11 ViewModels criados com validações específicas

4. **BancoContext**
   - ✅ DbSets adicionados para todas as categorias

5. **Repositório Genérico**
   - ✅ `IProdutoRepositoryGeneric` criado
   - ✅ `ProdutoRepositoryGeneric` implementado
   - ✅ Registrado no `Program.cs`

6. **SkuService**
   - ✅ Atualizado para trabalhar com `IProdutoBase`
   - ✅ Verifica SKUs em produtos antigos e novos

7. **Helper de Conversão**
   - ✅ `ProdutoConverter` criado para migração de dados

### ⚠️ Pendente (Próximos Passos)

1. **Criar Migration**
   ```bash
   dotnet ef migrations add SepararProdutosPorCategoria
   dotnet ef database update
   ```

2. **Atualizar ADMController**
   - Criar actions específicas por categoria
   - Ou usar factory pattern para criar produtos baseado na categoria
   - Atualizar `AdicionarProdutoADM` para usar os novos ViewModels

3. **Atualizar Views**
   - Criar formulários específicos para cada categoria em `GerenciarProdutos.cshtml`
   - Atualizar listagem para trabalhar com os novos models

4. **Migração de Dados**
   - Script para converter `ProdutoModel` antigos para os novos models específicos
   - Preservar dados existentes durante a transição

5. **Atualizar Outros Controllers**
   - `HomeController` - para trabalhar com os novos models
   - `CarrinhoController` - atualizar referências
   - Outros controllers que usam produtos

## ⚠️ Notas Importantes

1. **Compatibilidade:** O `ProdutoModel` antigo foi mantido para não quebrar funcionalidades existentes
2. **Migração de Dados:** Será necessário criar um script de migração para converter produtos antigos
3. **Validações:** Cada model tem validações específicas para seus campos obrigatórios
4. **SKU:** A geração de SKU continua funcionando através da interface `IProdutoBase`

## 📁 Estrutura de Arquivos

```
Models/
├── Produtos/
│   ├── IProdutoBase.cs
│   ├── ExtintorModel.cs
│   ├── MangueiraModel.cs
│   ├── HidranteModel.cs
│   ├── AcessorioModel.cs
│   ├── EPIModel.cs
│   ├── EPRModel.cs
│   ├── EPCModel.cs
│   ├── PortaCortaFogoModel.cs
│   ├── SistemaFixoModel.cs
│   ├── SistemaDeteccaoModel.cs
│   ├── EquipamentoArMandadoModel.cs
│   └── ProdutoConverter.cs (helper para conversão)
└── ViewModels/
    └── Produtos/
        ├── ExtintorCreateViewModel.cs
        ├── MangueiraCreateViewModel.cs
        ├── HidranteCreateViewModel.cs
        ├── AcessorioCreateViewModel.cs
        ├── EPICreateViewModel.cs
        ├── EPRCreateViewModel.cs
        ├── EPCCreateViewModel.cs
        ├── PortaCortaFogoCreateViewModel.cs
        ├── SistemaFixoCreateViewModel.cs
        ├── SistemaDeteccaoCreateViewModel.cs
        └── EquipamentoArMandadoCreateViewModel.cs
```

