using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using ToughService.Models;
using ToughService.Repository;
using System.Linq;
using Microsoft.AspNetCore.Identity;
namespace ToughService.Controllers
{

    [Authorize(Roles = "Admin")]
    public class ADMController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IChamadoRepository _chamadoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ADMController(
       IProdutoRepository produtoRepository,
       IWebHostEnvironment webHostEnvironment,
       IChamadoRepository chamadoRepository,
       UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _webHostEnvironment = webHostEnvironment;
            _chamadoRepository = chamadoRepository;
            _userManager = userManager;
        }

    
        public IActionResult ADM()
        {
            return View();
        }

       

        [HttpGet]
        public async Task<IActionResult> AdmChamados(
            int? status, string tipo, string ordenarPor = "data-desc", string busca = null)
        {
            try
            {
                var chamadosQuery = (await _chamadoRepository.GetAllChamadosAsync()).AsEnumerable();

                // Filtros
                if (status.HasValue && Enum.IsDefined(typeof(StatusChamadoEnum), status.Value))
                {
                    chamadosQuery = chamadosQuery.Where(c => (int)c.Status == status.Value);
                }
                if (!string.IsNullOrEmpty(tipo))
                {
                    chamadosQuery = chamadosQuery.Where(c => c.TipoServico.Equals(tipo, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(busca))
                {
                    string termoBuscaLower = busca.ToLower();
                    chamadosQuery = chamadosQuery.Where(c =>
                        c.Id.ToString().Contains(termoBuscaLower) ||
                        (c.NomeCliente != null && c.NomeCliente.ToLower().Contains(termoBuscaLower)) ||
                        (c.User?.Email != null && c.User.Email.ToLower().Contains(termoBuscaLower)) ||
                        (c.Telefone != null && c.Telefone.Contains(termoBuscaLower)) ||
                        (c.Cidade != null && c.Cidade.ToLower().Contains(termoBuscaLower)) ||
                        (c.TipoServico != null && c.TipoServico.ToLower().Contains(termoBuscaLower))
                    );
                }

                // Ordenação
                chamadosQuery = ordenarPor?.ToLower() switch
                {
                    "data-asc" => chamadosQuery.OrderBy(c => c.DataSolicitacao),
                    _ => chamadosQuery.OrderByDescending(c => c.DataSolicitacao),
                };

                // Guarda filtros para a View
                ViewBag.FiltroStatus = status;
                ViewBag.FiltroTipo = tipo;
                ViewBag.FiltroOrdenarPor = ordenarPor;
                ViewBag.FiltroBusca = busca;

                var chamadosLista = chamadosQuery.ToList();

                // Estatísticas
                ViewBag.TotalChamados = chamadosLista.Count;
                ViewBag.ChamadosNovos = chamadosLista.Count(c => c.Status == StatusChamadoEnum.Novo);
                ViewBag.ChamadosAndamento = chamadosLista.Count(c => c.Status == StatusChamadoEnum.EmAndamento);
                ViewBag.ChamadosFinalizados = chamadosLista.Count(c => c.Status == StatusChamadoEnum.Finalizado);

                return View(chamadosLista);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO CARREGAR ADM CHAMADOS: {ex}");
                TempData["ErroStatus"] = "Ocorreu um erro ao carregar os chamados.";
                return View(new List<ChamadoModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatusChamado(int id, StatusChamadoEnum novoStatus)
        {
            if (id <= 0 || !Enum.IsDefined(typeof(StatusChamadoEnum), novoStatus))
            {
                TempData["ErroStatus"] = "Dados inválidos para atualização de status.";
                return RedirectToAction("AdmChamados");
            }
            try
            {
                await _chamadoRepository.UpdateStatusChamadoAsync(id, novoStatus);
                TempData["ShowSuccessMessage"] = $"Status do chamado #{id} atualizado para '{novoStatus}'!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO ATUALIZAR STATUS (ID: {id}): {ex.Message}");
                TempData["ErroStatus"] = $"Erro ao atualizar status: {ex.Message}";
            }
            return RedirectToAction("AdmChamados");
        }



       
        [HttpGet]
        public async Task<IActionResult> GerenciarProdutos()
        {
            var listaDeProdutos = await _produtoRepository.GetAllProdutosAsync();
            return View(listaDeProdutos);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirProduto(int id)
        {
         
            await _produtoRepository.RemoveProdutoAsync(id);
            TempData["SucessoFormProduto"] = "Produto excluído com sucesso.";
            return RedirectToAction("GerenciarProdutos");
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
