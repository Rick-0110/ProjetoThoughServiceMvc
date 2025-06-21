using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;

namespace ToughService.Controllers;

public class ServicosController : Controller
{
    private readonly ILogger<ServicosController> _logger;

    public ServicosController(ILogger<ServicosController> logger)
    {
        _logger = logger;
    }

    public IActionResult Servicos()
    {
        return View();
    }

  

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
