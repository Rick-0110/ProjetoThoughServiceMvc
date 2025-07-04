using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using System.Collections.Generic;

namespace ProjetoThoughServiceMvc.Controllers
{
    // Controlador responsável pelas operações do carrinho de compras
    public class CarrinhoController : Controller
    {
        // Chave usada para armazenar e recuperar o carrinho da sessão do usuário
        private const string CarrinhoSessionKey = "Carrinho";

        // Método que retorna a view com os itens atuais do carrinho
        public IActionResult Index()
        {
            // Tenta recuperar a lista de itens do carrinho da sessão,
            // se não existir, cria uma lista vazia
            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

            // Passa a lista para a view para exibição
            return View(carrinho);
        }

        // Método POST para adicionar um item ao carrinho
       [HttpPost]
public IActionResult Adicionar(int id, string nome, string preco)
{
    // Converte a string preco para decimal usando a cultura brasileira
    if (!decimal.TryParse(preco, System.Globalization.NumberStyles.Number, new System.Globalization.CultureInfo("pt-BR"), out decimal precoDecimal))
    {
        // Se não conseguir converter, redireciona ou lança erro
        return BadRequest("Preço inválido");
    }

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
            Preco = precoDecimal,
            Quantidade = 1
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
