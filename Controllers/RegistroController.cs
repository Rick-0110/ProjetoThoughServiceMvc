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
        [HttpGet]
        public IActionResult Lockout()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel registro, CancellationToken cancellationToken)
        {
            // ... (Lógica de Registro existente)
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
        // MÉTODOS DE AUTENTICAÇÃO EXTERNA (GOOGLE) - NOVOS
        // ----------------------------------------------------

        // 1. INICIA O FLUXO DE LOGIN EXTERNO
        // O `provider` será "Google"
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Requisita um redirecionamento para o provedor (Google)
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Registro", new { returnUrl });

            // Cria as propriedades de autenticação com o URL de retorno para o nosso Callback
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Inicia o desafio do provedor (redireciona para o Google)
            return Challenge(properties, provider);
        }

        // 2. RECEBE O RETORNO DO GOOGLE (CallbackPath = /Registro/Login)
        // Este método será chamado pelo ASP.NET Core após o Google autenticar o usuário
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                // Mensagem de erro se o Google rejeitou a autenticação
                ModelState.AddModelError(string.Empty, $"Erro do provedor externo: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            // Lê as informações de login externo que foram armazenadas temporariamente
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Não foi possível carregar a informação de login externo (algo deu errado)
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
                return RedirectToAction(nameof(Lockout)); // Você pode criar um método para bloqueio
            }
            else
            {
                // Usuário é novo ou está usando este provedor pela primeira vez. 
                // Precisamos registrar o novo usuário usando os dados do Google.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;

                // Tenta obter o e-mail do Google (o ClaimTypes.Email foi configurado no Program.cs)
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
                // 1. Tenta obter as informações de login externo novamente
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

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        await MigrarCarrinhoSessaoParaBD(user.Id);
                        return RedirectToLocal(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
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