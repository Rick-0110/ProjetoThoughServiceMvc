using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using ToughService.Models;
using Microsoft.AspNetCore.Hosting;

namespace ToughService.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _hostEnvironment;

    public PerfilController(UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
    {
        _userManager = userManager;
        _hostEnvironment = hostEnvironment;
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

    [HttpPost]
    public async Task<IActionResult> UploadProfilePicture(IFormFile arquivo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (arquivo != null && arquivo.Length > 0)
        {
           
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string uploadFolder = Path.Combine(wwwRootPath, "images", "profiles");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 3. Crie um nome de arquivo único
            // Usa o ID do usuário para garantir que cada usuário tenha um único arquivo
            string fileName = $"{user.Id}_{Path.GetFileName(arquivo.FileName)}";
            string filePath = Path.Combine(uploadFolder, fileName);

            // 4. Salve o arquivo no servidor
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // 5. Atualize o caminho no banco de dados
            user.ProfilePicturePath = $"/images/profiles/{fileName}";
            await _userManager.UpdateAsync(user);

            TempData["Sucesso"] = "Foto de perfil atualizada com sucesso!";
        }

        return RedirectToAction("Perfil"); // Redireciona para a página de perfil
    }
}
