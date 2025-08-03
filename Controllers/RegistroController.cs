using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models; // ApplicationUser
using ToughService.Models; // seus modelos de input (RegistroModel, LoginModel)

namespace ToughService.Controllers
{
    public class RegistroController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
private readonly SignInManager<ApplicationUser> _signInManager;

public RegistroController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
{
    _userManager = userManager;
    _signInManager = signInManager;
}

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel registro)
        {
            if (!ModelState.IsValid)
                return View(registro);

            var user = new ApplicationUser
            {
                UserName = registro.Email,
                Email = registro.Email,
                Nome = registro.Nome,
                CpfCnpj = registro.CpfCnpj,
                EhAdmin = registro.Email == "matosh0111@gmail.com"
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
                return RedirectToAction("Perfil", "Perfil");

            ModelState.AddModelError("", "Email ou senha inválidos.");
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Registro");
        }
    }
}
