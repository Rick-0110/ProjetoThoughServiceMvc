using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToughService.Models;

[Authorize] 
public class PerfilController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PerfilController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        // Obtém o ID do usuário logado
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Busca o usuário pelo ID
        var usuario = await _userManager.FindByIdAsync(userId);

        if (usuario == null)
            return RedirectToAction("Login", "Registro");

        return View(usuario); // Passa o ApplicationUser para a View
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarPerfil(ApplicationUser model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usuario = await _userManager.FindByIdAsync(userId);

        if (usuario == null)
            return RedirectToAction("Login", "Registro");

        // Atualiza apenas os campos permitidos
        usuario.Nome = model.Nome;
        usuario.CpfCnpj = model.CpfCnpj;

        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
            TempData["Mensagem"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("Index", usuario);
    }

    [HttpGet]
    public IActionResult VerHistoricoPedidos()
    {
        return View();
    }
}
