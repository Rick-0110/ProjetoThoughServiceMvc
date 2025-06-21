using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;

namespace ToughService.Controllers;

public class PerfilController : Controller
{
    private readonly ILogger<PerfilController> _logger;

    public PerfilController(ILogger<PerfilController> logger)
    {
        _logger = logger;
    }

    public IActionResult Perfil()
    {
        return View();
    }

  

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
