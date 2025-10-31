using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using ToughService.Models;
using ToughService.Repository;
using ToughService.Services;


namespace ToughService.Controllers
{
    public class RegistroController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICaptchaService _captchaService;
        private readonly IConfiguration _configuration;
        private readonly ICarrinhoRepository _carrinhoRepository; 
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public RegistroController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICaptchaService captchaService,
            IConfiguration configuration,
            ICarrinhoRepository carrinhoRepository, 
            IHttpContextAccessor httpContextAccessor) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _captchaService = captchaService;
            _configuration = configuration;
            _carrinhoRepository = carrinhoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            ViewData["SiteKey"] = _configuration["Captcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel registro, CancellationToken cancellationToken)
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

                await MigrarCarrinhoSessaoParaBD(user.Id);


                return RedirectToAction("Perfil", "Perfil");
            }

            foreach (var error in result.Errors)
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
                var user = await _userManager.FindByEmailAsync(login.Email);
                await MigrarCarrinhoSessaoParaBD(user.Id);


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
            await _signInManager.SignOutAsync();
            _httpContextAccessor.HttpContext.Session.Remove("Carrinho");
            return RedirectToAction("Login", "Registro");
        }
        private async Task MigrarCarrinhoSessaoParaBD(string userId)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var carrinhoSessao = session.GetObject<List<ItemCarrinhoModel>>("Carrinho");

            if (carrinhoSessao != null && carrinhoSessao.Any())
            {
                var dbCarrinho = await _carrinhoRepository.GetCarrinhoByUserIdAsync(userId);

                foreach (var itemSessao in carrinhoSessao)
                {
                    var itemExistente = dbCarrinho.FirstOrDefault(i => i.ProdutoId == itemSessao.ProdutoId);

                    if (itemExistente != null)
                    {
                        // Se existe, soma a quantidade
                        itemExistente.Quantidade += itemSessao.Quantidade;
                        await _carrinhoRepository.UpdateItemAsync(itemExistente);
                    }
                    else
                    {
                        // Se não existe, associa o UserId e adiciona à BD
                        itemSessao.UserId = userId;
                        await _carrinhoRepository.AddItemAsync(itemSessao);
                    }
                }
                session.Remove("Carrinho");
            }
        }
    }
}