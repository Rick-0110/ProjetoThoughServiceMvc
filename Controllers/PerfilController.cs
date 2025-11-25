using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToughService.Models;

namespace ToughService.Controllers;

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
        var usuario = await ObterUsuarioAtualAsync();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Registro");
        }

        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarPerfil(ApplicationUser model)
    {
        var usuario = await ObterUsuarioAtualAsync();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Registro");
        }

        // Atualiza apenas os campos permitidos
        usuario.Nome = model.Nome;
        usuario.CpfCnpj = model.CpfCnpj;

        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
            TempData["Mensagem"] = "Perfil atualizado com sucesso!";
            return RedirectToAction(nameof(Perfil));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(nameof(Perfil), usuario);
    }

    [HttpGet]
    public IActionResult VerHistoricoPedidos()
    {
        return View();
    }

    private async Task<ApplicationUser?> ObterUsuarioAtualAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario != null)
        {
            return usuario;
        }

        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(email))
        {
            usuario = await _userManager.FindByEmailAsync(email);
            if (usuario != null)
            {
                return usuario;
            }
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            return await _userManager.FindByIdAsync(userId);
        }

        return null;
    }
}
