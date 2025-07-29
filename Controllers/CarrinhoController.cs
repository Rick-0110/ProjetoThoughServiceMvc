using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using System.Collections.Generic;
using ToughService.Data;

namespace ProjetoThoughServiceMvc.Controllers
{
    // Controlador responsável pelas operações do carrinho de compras
    public class CarrinhoController : Controller
    {
        private readonly BancoContext _context;
            public CarrinhoController(BancoContext context)
    {
        _context = context;
    }

    public IActionResult InfoProduto(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
        if (produto == null)
        {
            return NotFound();
        }
        return View(produto);
    }

        // Chave usada para armazenar e recuperar o carrinho da sessão do usuário
        private const string CarrinhoSessionKey = "Carrinho";

        // Método que retorna a view com os itens atuais do carrinho
        public IActionResult Index()
        {
            
           var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

            // Calcula o subtotal, frete e total dos itens no carrinho
            var subtotal = 0m;
            foreach (var item in carrinho)
            {
                subtotal += item.Preco * item.Quantidade;
            }

            var frete = 0m; // Aqui você pode implementar a lógica de cálculo de frete

            if (subtotal > 100)
            {
                frete = 0; // Frete grátis para compras acima de R$100
            }
            else
            {
                frete = 10; // Frete fixo de R$10 para compras abaixo de R$100
            }
            var total = subtotal + frete;

            ViewBag.Subtotal = subtotal;
            ViewBag.Frete = frete;
            ViewBag.Total = total;

            return View(carrinho);
        }




        // Método POST para adicionar um item ao carrinho



        [HttpPost]
public IActionResult Adicionar(int id, int quantidade)
{
    var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null)
    {
        return NotFound("Produto não encontrado.");
    }

    var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

    var itemExistente = carrinho.FirstOrDefault(i => i.Id == id);
    if (itemExistente != null)
    {
        itemExistente.Quantidade += quantidade;
    }
    else
    {
        carrinho.Add(new ItemCarrinhoModel
        {
            Id = produto.Id,
            NomeProduto = produto.Nome,
            Preco = produto.Preco,
            Quantidade = quantidade
        });
    }

    HttpContext.Session.SetObject(CarrinhoSessionKey, carrinho);

    return RedirectToAction("Index");
}



        // Método POST para limpar o carrinho, removendo todos os itens
        [HttpPost]
        public IActionResult Limpar()
        {
            // Remove a chave do carrinho da sessão, esvaziando-o
            HttpContext.Session.Remove(CarrinhoSessionKey);

            // Redireciona para a página do carrinho, que agora estará vazio
            return RedirectToAction("Index");
        }
    }
}
