using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoThoughServiceMvc.Models;
using System.Collections.Generic;
using ToughService.Data;
using ToughService.Models;
using ToughService.Repository;

namespace ProjetoThoughServiceMvc.Controllers
{
    
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;

        public CarrinhoController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }


    

        
        private const string CarrinhoSessionKey = "Carrinho";

        
        public IActionResult Index()
        {
            
           var carrinho = HttpContext.Session.GetObject<List<ItemCarrinhoModel>>(CarrinhoSessionKey) ?? new List<ItemCarrinhoModel>();

           
            var subtotal = 0m;
            foreach (var item in carrinho)
            {
                subtotal += item.Preco * item.Quantidade;
            }

            var frete = 0m;

            if (subtotal > 100)
            {
                frete = 0; 
            }
            else
            {
                frete = 10; 
            }
            var total = subtotal + frete;

            ViewBag.Subtotal = subtotal;
            ViewBag.Frete = frete;
            ViewBag.Total = total;

            return View(carrinho);
        }




     



        [HttpPost]
        public IActionResult Adicionar(int id, int quantidade)
        {
                 if (!User.Identity.IsAuthenticated)
            {
        return RedirectToAction("Login", "Registro");
    }

            var produto = _produtoRepository.GetProdutoById(id);
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

        // Remover o método duplicado InfoProduto(int id)
        // O método InfoProduto aparece duas vezes no mesmo controller, causando o erro CS0111.
        // Mantenha apenas uma versão do método. Aqui está a versão mais completa, que retorna outros produtos também:

        public IActionResult InfoProduto(int id)
        {
            var produto = _produtoRepository.GetProdutoById(id);

            if (produto == null)
                return NotFound();

            // Buscar outros produtos no repositório
            var outrosProdutos = _produtoRepository
                .GetAllProdutos()
                .Where(p => p.Id != id)
                .Take(4) // mostra até 4
                .ToList();

            var viewModel = new ProdutoDetalheViewModel
            {
                Produto = produto,
                OutrosProdutos = outrosProdutos
            };

            return View(viewModel);
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
