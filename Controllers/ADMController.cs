using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using ToughService.Models;
using ToughService.Repository;

namespace ToughService.Controllers
{

    [Authorize(Roles = "Admin")]
    public class ADMController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ADMController(IProdutoRepository produtoRepository, IWebHostEnvironment webHostEnvironment)
        {
            _produtoRepository = produtoRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult ADM()
        {

            return View();
        }

        public IActionResult AdmChamados()
        {

            return View();
        }



        public IActionResult AdicionarProdutoADM()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ListarProdutos()
        {
            try
            {
                var produtos = _produtoRepository.GetAllProdutosAsync();
                return Ok(produtos);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao buscar os produtos.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterProduto(int id)
        {
            var produto = _produtoRepository.GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpDelete]
        public IActionResult DeletarProduto(int id)
        {
            var produto = _produtoRepository.GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            _produtoRepository.RemoveProdutoAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] ProdutoModel produto)
        {
            if (id != produto.Id)
            {
                return BadRequest("Dados do produto inválidos.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produtoExistente = await _produtoRepository.GetProdutoByIdAsync(id);
            if (produtoExistente == null)
            {
                return NotFound("Produto não encontrado.");
            }

            produtoExistente.Nome = produto.Nome;
            produtoExistente.Descricao = produto.Descricao;
            produtoExistente.Preco = produto.Preco;
            produtoExistente.Categoria = produto.Categoria;
            produtoExistente.Sku = produto.Sku;
            produtoExistente.Marca = produto.Marca;
            produtoExistente.Quantidade = produto.Quantidade;
            produtoExistente.Peso = produto.Peso;
            produtoExistente.Ativo = produto.Ativo;
            var updatedProduto = _produtoRepository.UpdateProdutoAsync(produtoExistente);
            return Ok(updatedProduto);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarProdutoADM(ProdutoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var listaDeProdutos = await _produtoRepository.GetAllProdutosAsync();
                return View("GerenciarProdutos", listaDeProdutos);
            }

            CategoriaEnum categoriaConvertida;
            bool conversaoBemSucedida = Enum.TryParse<CategoriaEnum>(model.Categoria, true, out categoriaConvertida);

            if (!conversaoBemSucedida)
            {
                ModelState.AddModelError("Categoria", "Categoria inválida.");
                var listaDeProdutos = await _produtoRepository.GetAllProdutosAsync();
                return View("GerenciarProdutos", listaDeProdutos);
            }

            // Criação do novoProduto a partir do model recebido
            var novoProduto = new ProdutoModel
            {
                Nome = model.Nome,
                Descricao = model.Descricao,
                Preco = model.Preco,
                Categoria = categoriaConvertida,
                Sku = model.Sku,
                Marca = model.Marca,
                Quantidade = model.Quantidade,
                Peso = model.Peso,
                Ativo = model.Ativo
            };

            await _produtoRepository.AddProdutoAsync(novoProduto);

         
            return RedirectToAction("GerenciarProdutos");
        }

        // Crie esta nova Action [HttpPost] para lidar com a atualização do produto.
        [HttpPost]
        public async Task<IActionResult> AtualizarProduto(ProdutoModel model)
        {

            if (ModelState.IsValid)
            {
                var produtoParaAtualizar = await _produtoRepository.GetProdutoByIdAsync(model.Id);
                if (produtoParaAtualizar != null)
                {
                    // Mapeie os dados do model para a entidade
                    produtoParaAtualizar.Nome = model.Nome;
                    produtoParaAtualizar.Descricao = model.Descricao;
                    produtoParaAtualizar.Preco = model.Preco;
                    produtoParaAtualizar.Quantidade = model.Quantidade;
                    produtoParaAtualizar.Categoria = model.Categoria;
                   

                    await _produtoRepository.UpdateProdutoAsync(produtoParaAtualizar);
                }
            }

            // Após atualizar (ou se o modelo for inválido), redirecione para a página principal.
            return RedirectToAction("GerenciarProdutos");
        }

    }
    }
