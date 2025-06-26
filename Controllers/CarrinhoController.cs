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
        public IActionResult Adicionar(int id, string nome, decimal preco)
        {
            // Recupera o carrinho da sessão ou inicializa um novo
            var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

            // Verifica se o item já existe no carrinho
            var itemExistente = carrinho.Find(i => i.Id == id);
            if (itemExistente != null)
            {
                // Se existir, apenas incrementa a quantidade
                itemExistente.Quantidade++;
            }
            else
            {
                // Se não existir, cria um novo item com quantidade 1
                carrinho.Add(new ItemCarrinhoModel
                {
                    Id = id,
                    NomeProduto = nome,
                    Preco = preco,
                    Quantidade = 1
                });
            }

            // Atualiza o carrinho na sessão com a lista modificada
            HttpContext.Session.SetObject(CarrinhoSessionKey, carrinho);

            // Redireciona o usuário para a página do carrinho para ver as alterações
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
