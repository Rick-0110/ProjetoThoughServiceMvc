using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ToughService.Models;
using ToughService.Services;
using ToughService.Extensions;
using ToughService.Repository;
using System.Linq;
using System.Security.Claims;

namespace ToughService.Controllers
{
    public class RegistroController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICaptchaService _captchaService;
        private readonly IConfiguration _configuration;
        private readonly ICarrinhoRepository _carrinhoRepository;

        public RegistroController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ICaptchaService captchaService, 
            IConfiguration configuration,
            ICarrinhoRepository carrinhoRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _captchaService = captchaService;
            _configuration = configuration;
            _carrinhoRepository = carrinhoRepository;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            ViewData["SiteKey"] = _configuration["Captcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel registro,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(registro);

            var captchaToken = Request.Form["g-recaptcha-response"];
            var captchaValido = await _captchaService.VerifyCaptchaAsync(captchaToken, cancellationToken);

            var user = new ApplicationUser
            {
                UserName = registro.Email,
                Email = registro.Email,
                Nome = registro.Nome,
                CpfCnpj = registro.CpfCnpj,
               
            };

            var result = await _userManager.CreateAsync(user, registro.Senha);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Perfil", "Perfil");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registro);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Senha, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Carregar carrinho do banco de dados para a sessão
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    var carrinhoItems = await _carrinhoRepository.ObterItensPorUsuarioAsync(user.Id);
                    
                    // Converter CarrinhoItem para ItemCarrinhoModel (modelo da sessão)
                    var carrinhoSessao = carrinhoItems.Select(ci => new ItemCarrinhoModel
                    {
                        Id = ci.ProdutoId,
                        NomeProduto = ci.NomeProduto,
                        Preco = ci.Preco,
                        Quantidade = ci.Quantidade,
                        ImagemUrl = ci.ImagemUrl
                    }).ToList();

                    HttpContext.Session.SetObject("Carrinho", carrinhoSessao);
                }

                return RedirectToAction("Perfil", "Perfil");
            }

            ModelState.AddModelError("", "Email ou senha inválidos.");
            return View(login);
        }

        [HttpGet]
        public IActionResult EsqueciMinhaSenha()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (!string.IsNullOrEmpty(userId))
                {
                    // Salvar carrinho da sessão no banco de dados antes de fazer logout
                    var carrinhoSessao = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>("Carrinho") ?? new List<ItemCarrinhoModel>();
                    
                    if (carrinhoSessao.Any())
                    {
                        // Limpar carrinho antigo do banco
                        await _carrinhoRepository.RemoverItensPorUsuarioAsync(userId);
                        
                        // Salvar cada item do carrinho no banco
                        foreach (var item in carrinhoSessao)
                        {
                            var carrinhoItem = new CarrinhoItem
                            {
                                UserId = userId,
                                ProdutoId = item.Id,
                                NomeProduto = item.NomeProduto,
                                Preco = item.Preco,
                                Quantidade = item.Quantidade,
                                ImagemUrl = item.ImagemUrl,
                                DataAdicionado = DateTime.Now
                            };
                            
                            await _carrinhoRepository.AdicionarItemAsync(carrinhoItem);
                        }
                    }
                }
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Registro");
        }
    }
}
