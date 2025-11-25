using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Repository;
using ToughService.Services;
using ToughService.Extensions;

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
        private readonly ILogger<RegistroController> _logger; // Adicionado para logs

        public RegistroController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICaptchaService captchaService,
            IConfiguration configuration,
            ICarrinhoRepository carrinhoRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RegistroController> logger) // Logger injetado
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _captchaService = captchaService;
            _configuration = configuration;
            _carrinhoRepository = carrinhoRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger; // Inicialização do logger
        }

        [HttpGet]
        public IActionResult Registro()
        {
            ViewData["SiteKey"] = _configuration["Captcha:SiteKey"];
            return View();
        }
        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel registro, CancellationToken cancellationToken)
        {
            // ... (Lógica de Captcha e validação omitida para foco)

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

            // Se falhar, registra os erros para debug
            _logger.LogError("Falha ao registrar usuário padrão: Email={Email}", registro.Email);
            foreach (var error in result.Errors)
            {
                _logger.LogError("  - Erro Identity: {Code} - {Description}", error.Code, error.Description);
                ModelState.AddModelError("", error.Description);
            }

            ViewData["SiteKey"] = _configuration["Captcha:SiteKey"];
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
            // ... (Lógica de Login existente)
            if (!ModelState.IsValid)
                return View(login);

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Senha, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user != null)
                {
                    await MigrarCarrinhoSessaoParaBD(user.Id);
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
            await _signInManager.SignOutAsync();
            _httpContextAccessor.HttpContext.Session.Remove("Carrinho");
            return RedirectToAction("Login", "Registro");
        }


        // ----------------------------------------------------
        // MÉTODOS DE AUTENTICAÇÃO EXTERNA (GOOGLE)
        // ----------------------------------------------------

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Registro", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            // ... (Código para tratamento de erro e obtenção de info omitido)

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Erro ao carregar informações de login externo.");
                return RedirectToAction(nameof(Login));
            }

            // Tenta logar o usuário se ele já tiver se registrado com este provedor
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                // Login bem-sucedido (usuário existente)
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user != null)
                {
                    await MigrarCarrinhoSessaoParaBD(user.Id);
                }
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // Usuário é novo ou está usando este provedor pela primeira vez.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nome = info.Principal.FindFirstValue(ClaimTypes.Name);

                // Redireciona para uma tela de confirmação de e-mail e registro final
                return View("ExternalLoginConfirmation", new ExternalLoginModel
                {
                    Email = email,
                    Nome = nome
                });
            }
        }

        // 3. CONFIRMAÇÃO E REGISTRO DO NOVO USUÁRIO GOOGLE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao carregar informações de login externo.");
                    return View(nameof(Login));
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome
                };


                var createResult = await _userManager.CreateAsync(user);

                if (createResult.Succeeded)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);

                    if (addLoginResult.Succeeded)
                    {
                        // SUCESSO COMPLETO: Usuário salvo, Login externo linkado e autenticação local feita.
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        await MigrarCarrinhoSessaoParaBD(user.Id);
                        return RedirectToLocal(returnUrl);
                    }

                    // 🛑 FALHA no AddLogin (Raro, mas possível)
                    // Se falhar aqui, o usuário foi criado, mas não linkado ao Google.
                    _logger.LogError("Falha ao adicionar login externo para {Email}.", user.Email);
                    createResult = addLoginResult; // Usa os erros de AddLogin
                }

                // Trata erros de CreateAsync ou AddLoginAsync
                _logger.LogError("Falha ao registrar/linkar login externo: Email={Email}", model.Email);
                foreach (var error in createResult.Errors)
                {
                    _logger.LogError("  - Erro Identity: {Code} - {Description}", error.Code, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("ExternalLoginConfirmation", model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Perfil", "Perfil");
            }
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
                        itemExistente.Quantidade += itemSessao.Quantidade;
                        await _carrinhoRepository.UpdateItemAsync(itemExistente);
                    }
                    else
                    {
                        itemSessao.UserId = userId;
                        await _carrinhoRepository.AddItemAsync(itemSessao);
                    }
                }
                session.Remove("Carrinho");
            }
        }
    }
}