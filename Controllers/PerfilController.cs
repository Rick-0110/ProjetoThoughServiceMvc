using Microsoft.AspNetCore.Mvc;
using ToughService.Data;
using ToughService.Models;
using Microsoft.AspNetCore.Http;

public class PerfilController : Controller
{
    private readonly BancoContext _context;

    public PerfilController(BancoContext context)
    {
        _context = context;
    }

    // GET: Mostrar perfil
    [HttpGet]
    public IActionResult Perfil()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Registro");

        var usuario = _context.Usuarios.Find(userId.Value);
        if (usuario == null)
            return RedirectToAction("Login", "Registro");

        return View(usuario);
    }

    // POST: Atualizar perfil
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AtualizarPerfil(UsuarioModel model)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Registro");

        var usuario = _context.Usuarios.Find(userId.Value);
        if (usuario == null)
            return RedirectToAction("Login", "Registro");

        // Atualizar os campos Nome e CPF/CNPJ
        usuario.Nome = model.Nome;
        usuario.CpfCnpj = model.CpfCnpj;

        _context.SaveChanges();

        TempData["Mensagem"] = "Perfil atualizado com sucesso!";

        return RedirectToAction("Perfil");
    }
}
