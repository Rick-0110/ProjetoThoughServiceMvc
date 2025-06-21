using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Data;
namespace ToughService.Controllers;
using Microsoft.EntityFrameworkCore;
using ToughService.Models;
using ToughService.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
public class RegistroController : Controller
{
    private readonly BancoContext _context;

    public RegistroController(BancoContext context)
    {
        _context = context;
    }

    // Mostrar formulário de registro
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    // Receber dados de registro
    [HttpPost]
    public IActionResult Registro(RegistroModel registro)
    {
        if (ModelState.IsValid)
        {
            // Verificar se email já existe
            if (_context.Usuarios.Any(u => u.Email == registro.Email))
            {
                ModelState.AddModelError("Email", "Email já cadastrado.");
                return View(registro);
            }

            // Criar usuário
            var usuario = new UsuarioModel
            {
                Nome = registro.Nome,
                CpfCnpj = registro.CpfCnpj,
                Email = registro.Email,
                Senha = registro.Senha // IMPORTANTE: usar hash em produção!
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            // Redirecionar para login após salvar
            return RedirectToAction("Login");
        }

        return View(registro);
    }

    // Mostrar formulário de login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Receber dados do login
    [HttpPost]
    public IActionResult Login(LoginModel login)
    {
        if (!ModelState.IsValid)
            return View(login);

        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.Senha == login.Senha);
        if (usuario != null)
        {
            // Salva o ID do usuário na sessão
            HttpContext.Session.SetInt32("UserId", usuario.Id);

            return RedirectToAction("Perfil", "Perfil");
        }

        ModelState.AddModelError("", "Email ou senha inválidos.");
        return View(login);
    }

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Logout()
{
    HttpContext.Session.Clear(); // Limpa toda sessão
    return RedirectToAction("Login", "Registro");
}



}
