using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToughService.Extensions;
using ToughService.Models;
using ToughService.Repository;
namespace ToughService.ViewComponents
{
    public class CarrinhoResumoViewComponent : ViewComponent
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarrinhoResumoViewComponent(
            ICarrinhoRepository carrinhoRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _carrinhoRepository = carrinhoRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int quantidadeItens = 0;
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                // 1. Utilizador Logado: Busca na Base de Dados
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var carrinhoDb = await _carrinhoRepository.GetCarrinhoByUserIdAsync(userId);
                quantidadeItens = carrinhoDb.Sum(i => i.Quantidade);
            }
            else
            {
                var carrinhoSessao = httpContext.Session.GetObject<List<ItemCarrinhoModel>>("Carrinho");
                if (carrinhoSessao != null)
                {
                    quantidadeItens = carrinhoSessao.Sum(i => i.Quantidade);
                }
            }

            return View(quantidadeItens);
        }
    }
}

