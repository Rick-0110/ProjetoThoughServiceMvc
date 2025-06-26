using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using System.Collections.Generic;

namespace ProjetoThoughServiceMvc.Controllers
{
    public class CarrinhoController : Controller
    {
        private const string CarrinhoSessionKey = "Carrinho";

        public IActionResult Index()
        {
            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();
            return View(carrinho);
        }

        [HttpPost]
        public IActionResult Adicionar(int id, string nome, decimal preco)
        {
            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

            var itemExistente = carrinho.Find(i => i.Id == id);
            if (itemExistente != null)
            {
                itemExistente.Quantidade++;
            }
            else
            {
                carrinho.Add(new ItemCarrinhoModel
                {
                    Id = id,
                    NomeProduto = nome,
                    Preco = preco,
                    Quantidade = 1
                });
            }

            HttpContext.Session.SetObject(CarrinhoSessionKey, carrinho);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Limpar()
        {
            HttpContext.Session.Remove(CarrinhoSessionKey);
            return RedirectToAction("Index");
        }
    }
}
