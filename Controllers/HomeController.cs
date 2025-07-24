using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ProjetoThoughServiceMvc.Models;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using ToughService.Data;
namespace ToughService.Controllers;

public class HomeController : Controller
{
     private readonly BancoContext _context;

    public HomeController(BancoContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var produtos = _context.Produtos.ToList();
        return View(produtos);
    }

    public IActionResult Privacy()
    {
        return View();
    }
      public IActionResult Sobre()
    {
        return View();
    }
    

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
