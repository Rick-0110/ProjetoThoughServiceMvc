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

        
        [HttpGet]
        public IActionResult AdicionarProdutoADM() 
        {
            return View();
        }

        // Action [HttpPost] para RECEBER os dados do formulário e SALVAR o produto
        [HttpPost]
        public async Task<IActionResult> AdicionarProdutoADM([FromForm] ProdutoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

       
            CategoriaEnum categoriaConvertida;
            bool conversaoOk = Enum.TryParse<CategoriaEnum>(model.Categoria, true, out categoriaConvertida);

            if (!conversaoOk)
            {
                ModelState.AddModelError("Categoria", "A categoria selecionada é inválid.");
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
                imagemUrlUnica = Guid.NewGuid().ToString() + "_" + model.Imagem.FileName;
                string caminhoArquivo = Path.Combine(pastaUploads, imagemUrlUnica);
                using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await model.Imagem.CopyToAsync(fileStream);
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

            _produtoRepository.AddProduto(novoProduto);

            return Ok(new { success = true, message = "Produto adicionado com sucesso!" });
        }
    }
}