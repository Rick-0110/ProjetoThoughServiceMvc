using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;

namespace ToughService.Controllers;

public class AreadecompraController : Controller
{
    private readonly ILogger<AreadecompraController> _logger;

    public AreadecompraController(ILogger<AreadecompraController> logger)
    {
        _logger = logger;
    }

    public IActionResult Areadecompra()
    {
        return View();
    }

  

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
