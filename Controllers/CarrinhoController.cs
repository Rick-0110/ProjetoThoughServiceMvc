using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;

namespace ToughService.Controllers;

public class CarrinhoController : Controller
{
    private readonly ILogger<CarrinhoController> _logger;

    public CarrinhoController(ILogger<CarrinhoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Carrinho()
    {
        return View();
    }

  

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
