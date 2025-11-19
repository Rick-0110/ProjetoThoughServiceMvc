using Microsoft.AspNetCore.Mvc;
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
    }
}
