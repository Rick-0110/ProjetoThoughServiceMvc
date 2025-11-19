using Microsoft.AspNetCore.Mvc;
using ToughService.Repository;
using ToughService.Dtos;


namespace ToughService.Controllers.Api
{
    [ApiController]
    [Route("api/estoque")]
    public class EstoqueApiController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;

        public EstoqueApiController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }



        [HttpGet("disponivel/{id:int}")]
        public async Task<IActionResult> GetEstoque(int id)
        {
            var produto = await _produtoRepository.GetProdutoByIdAsync(id);

            if (produto == null)
                return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });

            return Ok(new
            {
                ProdutoId = produto.Id,
                ProdutoNome = produto.Nome,
                QuantidadeDisponivel = produto.Quantidade
            });
        }

        [HttpPost("receber")]
        public async Task<IActionResult> ReceberEstoque([FromBody] EstoqueRecebimentoDto recebimento)
        {
            if (recebimento == null)
                return BadRequest("Dados inválidos.");

            var produto = await _produtoRepository.GetProdutoByIdAsync(recebimento.ProdutoId);

            if (produto == null)
                return NotFound(new { mensagem = "Produto não encontrado." });

            produto.Quantidade += recebimento.QuantidadeRecebida;

            await _produtoRepository.UpdateProdutoAsync(produto);

            return Ok(new
            {
                mensagem = "Estoque atualizado com sucesso!",
                produtoId = produto.Id,
                novaQuantidade = produto.Quantidade
            });
        }

        [HttpPost("baixar")]
        public async Task<IActionResult> DarBaixa([FromBody] EstoqueBaixoDto dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            var produto = await _produtoRepository.GetProdutoByIdAsync(dto.ProdutoId);

            if (produto == null)
                return NotFound(new { mensagem = "Produto não encontrado." });

            if (produto.Quantidade < dto.QuantidadeBaixa)
                return BadRequest(new { mensagem = "Estoque insuficiente!" });

            produto.Quantidade -= dto.QuantidadeBaixa;

            await _produtoRepository.UpdateProdutoAsync(produto);

            return Ok(new
            {
                mensagem = "Baixa de estoque realizada!",
                produtoId = produto.Id,
                novaQuantidade = produto.Quantidade
            });
        }

        [HttpGet("baixo")]
        public async Task<IActionResult> EstoqueBaixo([FromQuery] int limite = 5)
        {
            var produtos = await _produtoRepository.GetAllProdutosAsync();

            var criticos = produtos
                .Where(p => p.Quantidade <= limite)
                .Select(p => new
                {
                    p.Id,
                    p.Nome,
                    p.Quantidade
                })
                .ToList();

            return Ok(criticos);
        }



        [HttpGet("lista")]
        public async Task<IActionResult> ListarEstoque()
        {
                       var produtos = await _produtoRepository.GetAllProdutosAsync();
            var lista = produtos.Select(p => new
            {
                p.Id,
                p.Nome,
                p.Quantidade
            });
            return Ok(lista);
        }

    }
}
