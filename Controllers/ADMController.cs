using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ToughService.Models;
using ToughService.Repository;
namespace ToughService.Controllers
{

    [Authorize(Roles = "Admin")]
    public class ADMController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IChamadoRepository _chamadoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ADMController(
            IProdutoRepository produtoRepository,
            IWebHostEnvironment webHostEnvironment,
            IChamadoRepository chamadoRepository,
            IPedidoRepository pedidoRepository,
            UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _webHostEnvironment = webHostEnvironment;
            _chamadoRepository = chamadoRepository;
            _pedidoRepository = pedidoRepository;
            _userManager = userManager;
        }

    
        public async Task<IActionResult> ADM()
        {
            try
            {
                int totalChamados = (await _chamadoRepository.GetAllChamadosAsync()).Count();

                int totalProdutos = (await _produtoRepository.GetAllProdutosAsync()).Count();

                int totalPedidos = (await _pedidoRepository.GetAllPedidosAsync()).Count();

                var viewModel = new AdminDashboardViewModel
                {
                    TotalChamados = totalChamados,
                    TotalProdutosEmEstoque = totalProdutos,
                    TotalPedidos = totalPedidos
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO CARREGAR DADOS DO DASHBOARD: {ex.Message}");
                TempData["ErroStatus"] = "Ocorreu um erro ao carregar as estatísticas do dashboard.";
                return View(new AdminDashboardViewModel()); 
            }
        }
        

        [HttpGet]
        public async Task<IActionResult> GerenciarPedidos(
            int? status,
            string ordenarPor = "data-desc",
            string? busca = null)
        {
            try
            {
                var todosPedidos = (await _pedidoRepository.GetAllPedidosAsync()).ToList();

                IEnumerable<PedidoModel> pedidosFiltrados = todosPedidos;

                if (status.HasValue && Enum.IsDefined(typeof(StatusPedidoEnum), status.Value))
                {
                    pedidosFiltrados = pedidosFiltrados.Where(p => (int)p.Status == status.Value);
                    ViewBag.FiltroStatus = status.Value;
                }
                else
                {
                    ViewBag.FiltroStatus = null;
                }

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    var termo = busca.Trim().ToLowerInvariant();
                    pedidosFiltrados = pedidosFiltrados.Where(p =>
                        p.Id.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrWhiteSpace(p.NomeCliente) && p.NomeCliente.ToLowerInvariant().Contains(termo)) ||
                        (!string.IsNullOrWhiteSpace(p.EmailCliente) && p.EmailCliente.ToLowerInvariant().Contains(termo)) ||
                        (!string.IsNullOrWhiteSpace(p.TelefoneCliente) && p.TelefoneCliente.Contains(termo)) ||
                        (!string.IsNullOrWhiteSpace(p.Cidade) && p.Cidade.ToLowerInvariant().Contains(termo)));
                }

                pedidosFiltrados = ordenarPor?.ToLowerInvariant() switch
                {
                    "data-asc" => pedidosFiltrados.OrderBy(p => p.DataPedido),
                    "total-desc" => pedidosFiltrados.OrderByDescending(p => p.Total),
                    "total-asc" => pedidosFiltrados.OrderBy(p => p.Total),
                    _ => pedidosFiltrados.OrderByDescending(p => p.DataPedido)
                };

                var listaParaView = pedidosFiltrados.ToList();

                ViewBag.FiltroOrdenarPor = ordenarPor;
                ViewBag.FiltroBusca = busca;

                ViewBag.TotalPedidos = todosPedidos.Count;
                ViewBag.PedidosPendentes = todosPedidos.Count(p => p.Status == StatusPedidoEnum.Pendente);
                ViewBag.PedidosConfirmados = todosPedidos.Count(p => p.Status == StatusPedidoEnum.Confirmado);
                ViewBag.PedidosEnviados = todosPedidos.Count(p => p.Status == StatusPedidoEnum.Enviado);
                ViewBag.PedidosEntregues = todosPedidos.Count(p => p.Status == StatusPedidoEnum.Entregue);
                ViewBag.TotalVendas = todosPedidos.Sum(p => p.Total);

                return View(listaParaView);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO CARREGAR PEDIDOS: {ex}");
                TempData["ErroStatus"] = "Ocorreu um erro ao carregar os pedidos.";
                return View(new List<PedidoModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatusPedido(int id, StatusPedidoEnum novoStatus)
        {
            if (id <= 0 || !Enum.IsDefined(typeof(StatusPedidoEnum), novoStatus))
            {
                TempData["ErroStatus"] = "Dados inválidos para atualização de status.";
                return RedirectToAction(nameof(GerenciarPedidos));
            }

            try
            {
                await _pedidoRepository.UpdateStatusPedidoAsync(id, novoStatus);
                TempData["ShowSuccessMessage"] = $"Status do pedido #{id} atualizado para '{novoStatus}'.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO ATUALIZAR STATUS DO PEDIDO (ID: {id}): {ex.Message}");
                TempData["ErroStatus"] = $"Erro ao atualizar status do pedido: {ex.Message}";
            }

            return RedirectToAction(nameof(GerenciarPedidos));
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


        [HttpGet]
        public IActionResult AdicionarProdutoADM()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarProdutoADM(ProdutoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Coleta todos os erros de validação
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                string mensagemErro = erros.Any() 
                    ? string.Join(" ", erros) 
                    : "Erro ao adicionar produto. Verifique os campos obrigatórios.";
                
                Console.WriteLine($"ERRO DE VALIDAÇÃO: {mensagemErro}");
                TempData["ErroStatus"] = mensagemErro;
                var listaDeProdutos = await _produtoRepository.GetAllProdutosAsync();
                return View("GerenciarProdutos", listaDeProdutos);
            }

            CategoriaEnum categoriaConvertida;
            // Mapeamento: Formulário usa 1-4, Enum usa 0-3
            // 1=Extintores(0), 2=SistemasFixos(2), 3=SistemasDeDeteccao(3), 4=Acessorios(1)
            categoriaConvertida = model.CategoriaId switch
            {
                1 => CategoriaEnum.Extintores,        // 0 no enum
                2 => CategoriaEnum.SistemasFixos,    // 2 no enum
                3 => CategoriaEnum.SistemasDeDeteccao, // 3 no enum
                4 => CategoriaEnum.Acessorios,        // 1 no enum
                _ => throw new ArgumentException("Categoria inválida.")
            };
            
            if (!Enum.IsDefined(typeof(CategoriaEnum), categoriaConvertida))
            {
                ModelState.AddModelError("CategoriaId", "Categoria inválida.");
                TempData["ErroStatus"] = "Categoria inválida selecionada.";
                var listaDeProdutos = await _produtoRepository.GetAllProdutosAsync();
                return View("GerenciarProdutos", listaDeProdutos);
            }

            string caminhoArquivo = "sem_imagem.png";

            if (model.Imagem != null)
            {
                string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "IMG");
                if (!Directory.Exists(pastaUploads))
                {
                    Directory.CreateDirectory(pastaUploads);
                }

                caminhoArquivo = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                string caminhoCompleto = Path.Combine(pastaUploads, caminhoArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await model.Imagem.CopyToAsync(stream);
                }
            }

            // Mapear CategoriaId para o valor correto do enum (0-3)
            int categoriaIdMapeado = model.CategoriaId switch
            {
                1 => 0, // Extintores
                2 => 2, // SistemasFixos
                3 => 3, // SistemasDeDeteccao
                4 => 1, // Acessorios
                _ => 0
            };

            var novoProduto = new ProdutoModel
            {
                Nome = model.Nome ?? string.Empty,
                Descricao = model.Descricao ?? string.Empty,
                Preco = model.Preco,
                CategoriaId = categoriaIdMapeado,
                Categoria = categoriaConvertida,
                Sku = model.Sku ?? string.Empty,
                Marca = model.Marca ?? string.Empty,
                Quantidade = model.Quantidade,
                Peso = model.Peso,
                Ativo = model.Ativo,
                ImagemUrl = caminhoArquivo
            };

            try
            {
                Console.WriteLine($"=== TENTANDO ADICIONAR PRODUTO ===");
                Console.WriteLine($"Nome: {novoProduto.Nome}");
                Console.WriteLine($"Descricao: {novoProduto.Descricao}");
                Console.WriteLine($"Preco: {novoProduto.Preco}");
                Console.WriteLine($"CategoriaId: {novoProduto.CategoriaId}");
                Console.WriteLine($"Categoria: {novoProduto.Categoria}");
                Console.WriteLine($"Quantidade: {novoProduto.Quantidade}");
                Console.WriteLine($"Ativo: {novoProduto.Ativo}");
                Console.WriteLine($"ImagemUrl: {novoProduto.ImagemUrl}");
                
                var produtoSalvo = await _produtoRepository.AddProdutoAsync(novoProduto);
                
                Console.WriteLine($"=== PRODUTO ADICIONADO COM SUCESSO! ===");
                Console.WriteLine($"ID Gerado: {produtoSalvo.Id}");
                Console.WriteLine($"Nome: {produtoSalvo.Nome}");
                
                TempData["SucessoFormProduto"] = $"Produto '{produtoSalvo.Nome}' adicionado com sucesso! (ID: {produtoSalvo.Id})";
            }
            // Adicione a captura de exceções do Entity Framework Core
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Esta exceção geralmente contém informações sobre violação de restrição do banco de dados (NOT NULL, tamanho, etc.)
                string erroDetalhado = dbEx.InnerException?.Message ?? dbEx.Message;
                Console.WriteLine($"ERRO DB: {erroDetalhado}");
                Console.WriteLine($"Stack Trace: {dbEx.StackTrace}");
                TempData["ErroStatus"] = $"Falha ao salvar no banco de dados. Detalhes: {erroDetalhado}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO GERAL: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                TempData["ErroStatus"] = $"Erro interno ao salvar o produto: {ex.Message}";
            }

            return RedirectToAction("GerenciarProdutos");
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarProduto(ProdutoModel model)
        {

            if (ModelState.IsValid)
            {
                var produtoParaAtualizar = await _produtoRepository.GetProdutoByIdAsync(model.Id);
                if (produtoParaAtualizar != null)
                {
                    // Mapeia os dados do model para a entidade
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarEstoque(int produtoId, int quantidade)
        {
            if (produtoId <= 0 || quantidade <= 0)
            {
                TempData["ErroStatus"] = "Dados inválidos para adicionar estoque.";
                return RedirectToAction("GerenciarProdutos");
            }

            try
            {
                var produto = await _produtoRepository.GetProdutoByIdAsync(produtoId);
                if (produto == null)
                {
                    TempData["ErroStatus"] = "Produto não encontrado.";
                    return RedirectToAction("GerenciarProdutos");
                }

                produto.Quantidade += quantidade;
                await _produtoRepository.UpdateProdutoAsync(produto);

                TempData["SucessoFormProduto"] = $"Estoque atualizado! {quantidade} unidade(s) adicionada(s) ao produto '{produto.Nome}'. Nova quantidade: {produto.Quantidade}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO ADICIONAR ESTOQUE (ID: {produtoId}): {ex.Message}");
                TempData["ErroStatus"] = $"Erro ao adicionar estoque: {ex.Message}";
            }

            return RedirectToAction("GerenciarProdutos");
        }

    }
    }
