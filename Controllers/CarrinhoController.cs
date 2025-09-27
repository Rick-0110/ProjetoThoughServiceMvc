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
                    Quantidade = quantidade,
                    ImagemUrl = produto.ImagemUrl
                });
            }

            HttpContext.Session.SetObject(CarrinhoSessionKey, carrinho);

            return RedirectToAction("Index");
       
}

        
        public IActionResult InfoProduto(int id)
        {
            var produto = _produtoRepository.GetProdutoById(id);

            if (produto == null)
                return NotFound();

            
            var outrosProdutos = _produtoRepository
                .GetAllProdutos()
                .Where(p => p.Id != id)
                .Take(4) 
                .ToList();

            var viewModel = new ProdutoDetalheViewModel
            {
                Produto = produto,
                OutrosProdutos = outrosProdutos
            };

            return View(viewModel);
        }




       
        [HttpPost]
        public IActionResult Limpar()
        {
            
            HttpContext.Session.Remove(CarrinhoSessionKey);

           
            return RedirectToAction("Index");
        }
    }
}
