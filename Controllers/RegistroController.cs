using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;

namespace ToughService.Controllers;

public class RegistroController : Controller
{
    private readonly ILogger<RegistroController> _logger;

    public RegistroController(ILogger<RegistroController> logger)
    {
        _logger = logger;
    }

    public IActionResult Registro()
    {
        return View();
    }

  

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
