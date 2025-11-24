using Microsoft.AspNetCore.Mvc;
using ToughService.Dtos;
using ToughService.Repository;



namespace ToughService.Controllers.Api
{
    [ApiController]
    [Route("api/carrinho")]
    public class CarrinhoApiController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        public CarrinhoApiController(
           ICarrinhoRepository carrinhoRepository,
           IProdutoRepository produtoRepository)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetCarrinho(string usuarioId)
        {
            var itens = await _carrinhoRepository.GetCarrinhoByUserIdAsync(usuarioId);
            return Ok(itens);

        }

        [HttpPost("adicionar")]
        public async Task<IActionResult> AddItem([FromBody] CarrinhoItemAddDto dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            var produto = await _produtoRepository.GetProdutoByIdAsync(dto.ProdutoId);

            if (produto == null)
                return NotFound("Produto não encontrado.");

            var userId = User?.Identity?.IsAuthenticated == true
            ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : null;

            if (userId == null)
                return Unauthorized("Usuário não autenticado.");

            await _carrinhoRepository.AddToCarrinhoAsync(
                dto.ProdutoId,
                dto.Quantidade,
                userId
            );

            return Ok(new
            {
                mensagem = "Item adicionado ao carrinho!",
                produtoId = dto.ProdutoId,
                quantidade = dto.Quantidade,
                usuario = userId
            });
        }

        [HttpDelete("remover/{produtoId}")]
        public async Task<IActionResult> RemoveItem(int produtoId)
        {
            var userId = User?.Identity?.IsAuthenticated == true
            ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : null;
            if (userId == null)
                return Unauthorized("Usuário não autenticado.");
            await _carrinhoRepository.RemoveItemAsync(produtoId, userId);
            return Ok(new
            {
                mensagem = "Item removido do carrinho!",
                produtoId = produtoId,
                usuario = userId
            });
        }
        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarCompra()
        {
            var userId = User?.Identity?.IsAuthenticated == true
            ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : null;
            if (userId == null)
                return Unauthorized("Usuário não autenticado.");
            await _carrinhoRepository.ClearCarrinhoAsync(userId);
            return Ok(new
            {
                mensagem = "Compra finalizada e carrinho limpo!",
                usuario = userId
            });
        }



    }
}
