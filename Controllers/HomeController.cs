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

   [HttpPost]
public IActionResult RemoverProduto(int id)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == userId);

    if (usuario == null || !usuario.EhAdmin)
        return RedirectToAction("Login", "Registro");

    var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);

    if (produto != null)
    {
        _context.Produtos.Remove(produto);
        _context.SaveChanges();
    }

    return RedirectToAction("Index");
}


 [HttpGet]
public IActionResult AdicionarProdutoADM()
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == userId);
    
    if (usuario == null || !usuario.EhAdmin)
        return RedirectToAction("Login", "Registro");

    return View(); // Mostra o formulário em branco
}
      public IActionResult Sobre()
    {
        return View();
    }

    [HttpPost]
public IActionResult AdicionarProdutoADM(ProdutoModel produto)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == userId);
    
    if (usuario == null || !usuario.EhAdmin)
        return RedirectToAction("Login", "Registro");

    if (ModelState.IsValid)
    {
        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    return View(produto);
}

    

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
