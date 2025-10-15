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
                var produtos =  _produtoRepository.GetAllProdutosAsync(); 
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
        public async Task<IActionResult> AtualizarProduto(int id,[FromBody] ProdutoModel produto)
        {
            if (id != produto.Id)
            {
                return BadRequest("Dados do produto inválidos.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produtoExistente =  await _produtoRepository.GetProdutoByIdAsync(id);
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
        [HttpPost]
        public async Task<IActionResult> AdicionarProdutoADM([FromForm] ProdutoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ... (resto do seu código para tratar categoria e imagem)
            CategoriaEnum categoriaConvertida;
            bool conversaoOk = Enum.TryParse<CategoriaEnum>(model.Categoria, true, out categoriaConvertida);

            if (!conversaoOk)
            {
                ModelState.AddModelError("Categoria", "A categoria selecionada é inválida.");
                return BadRequest(ModelState);
            }

            string imagemUrlUnica = null;
            if (model.Imagem != null && model.Imagem.Length > 0)
            {
                string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "images/produtos");
                if (!Directory.Exists(pastaUploads))
                {
                    Directory.CreateDirectory(pastaUploads);
                }
                imagemUrlUnica = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Imagem.FileName);
                string caminhoArquivo = Path.Combine(pastaUploads, imagemUrlUnica);
                using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await model.Imagem.CopyToAsync(fileStream); // A parte async está aqui
                }
            }

            var novoProduto = new ProdutoModel
            {
                Nome = model.Nome,
                Preco = model.Preco,
                Descricao = model.Descricao,
                ImagemUrl = imagemUrlUnica != null ? $"/images/produtos/{imagemUrlUnica}" : "/images/placeholder.png",
                Categoria = categoriaConvertida,
                Sku = model.Sku,
                Marca = model.Marca,
                Quantidade = model.Quantidade,
                Peso = model.Peso,
                Ativo = model.Ativo
            };

            // Chamada para o método síncrono do repositório
            _produtoRepository.AddProdutoAsync(novoProduto);

            return Ok(new { success = true, message = "Produto adicionado com sucesso!", produto = novoProduto });
        }
    }
}