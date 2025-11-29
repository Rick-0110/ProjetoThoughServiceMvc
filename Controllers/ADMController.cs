using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Models.Produtos;
using ToughService.Repository;
using ToughService.Services;

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
        private readonly ISkuService _skuService; 

        public ADMController(
            IProdutoRepository produtoRepository,
            IWebHostEnvironment webHostEnvironment,
            IChamadoRepository chamadoRepository,
            IPedidoRepository pedidoRepository,
            UserManager<ApplicationUser> userManager,
            ISkuService skuService) 
        {
            _produtoRepository = produtoRepository;
            _webHostEnvironment = webHostEnvironment;
            _chamadoRepository = chamadoRepository;
            _pedidoRepository = pedidoRepository;
            _userManager = userManager;
            _skuService = skuService;
        }

        // ---------------------------------------------------------------------------------------------------
        // DASHBOARD
        // ---------------------------------------------------------------------------------------------------

        public async Task<IActionResult> ADM()
        {
            try
            {
                var chamados = await _chamadoRepository.GetAllChamadosAsync();
                var produtos = await _produtoRepository.GetAllProdutosAsync();
                var pedidos = await _pedidoRepository.GetAllPedidosAsync();

                int totalChamados = chamados.Count();
                int totalProdutos = produtos.Count();
                int totalPedidos = pedidos.Count();


                var atividades = new List<AdminActivityViewModel>();


                atividades.AddRange(chamados.OrderByDescending(c => c.DataSolicitacao).Take(5).Select(c => new AdminActivityViewModel
                {
                    Tipo = "Chamado",
                    Mensagem = $"Novo chamado: {c.TipoServico} - {c.NomeCliente}",
                    Data = c.DataSolicitacao,
                    IconeCss = "fas fa-clipboard-list",
                    CorCss = "text-warning"
                }));


                atividades.AddRange(pedidos.OrderByDescending(p => p.DataPedido).Take(5).Select(p => new AdminActivityViewModel
                {
                    Tipo = "Pedido",
                    Mensagem = $"Novo pedido #{p.Id}: R$ {p.Total:F2}",
                    Data = p.DataPedido,
                    IconeCss = "fas fa-shopping-cart",
                    CorCss = "text-primary"
                }));

       
                atividades.AddRange(produtos.OrderByDescending(p => p.Id).Take(5).Select(p => new AdminActivityViewModel
                {
                    Tipo = "Produto",
                    Mensagem = $"Produto cadastrado: {p.Nome}",
                    Data = DateTime.Now, 
                    IconeCss = "fas fa-plus",
                    CorCss = "text-success"
                }));


                var atividadesFinais = atividades.OrderByDescending(a => a.Data).Take(10).ToList();

                var viewModel = new AdminDashboardViewModel
                {
                    TotalChamados = totalChamados,
                    TotalProdutosEmEstoque = totalProdutos,
                    TotalPedidos = totalPedidos,
                    AtividadesRecentes = atividadesFinais 
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO DASHBOARD: {ex.Message}");
                return View(new AdminDashboardViewModel());
            }
        }
        // ---------------------------------------------------------------------------------------------------
        // PEDIDOS
        // ---------------------------------------------------------------------------------------------------

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

        // ---------------------------------------------------------------------------------------------------
        // CHAMADOS
        // ---------------------------------------------------------------------------------------------------

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

        // ---------------------------------------------------------------------------------------------------
        // PRODUTOS (ADM)
        // ---------------------------------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GerenciarProdutos()
        {
            var listaDeProdutos = await _produtoRepository.GetAllProdutosAsync();
            
            // Agrupa produtos por categoria para exibição organizada
            var produtosPorCategoria = listaDeProdutos
                .GroupBy(p => p.Categoria)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            ViewBag.ProdutosPorCategoria = produtosPorCategoria;
            
            return View(listaDeProdutos);
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
            ModelState.Remove(nameof(model.Sku));

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErroStatus"] = $"Preencha corretamente: {string.Join(" ", erros)}";
                return View(model);
            }

            CategoriaEnum categoriaConvertida;
            try
            {
                categoriaConvertida = (CategoriaEnum)model.CategoriaId;
                if (!Enum.IsDefined(typeof(CategoriaEnum), categoriaConvertida))
                    throw new ArgumentException("Categoria inválida.");
            }
            catch
            {
                TempData["ErroStatus"] = "Categoria inválida.";
                return View(model);
            }

            string caminhoArquivo = "sem_imagem.png";
            if (model.Imagem != null)
            {
                try
                {
                   
                    string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "IMG", "Produtos");
                    if (!Directory.Exists(pastaUploads)) Directory.CreateDirectory(pastaUploads);

                    caminhoArquivo = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                    var caminhoCompleto = Path.Combine(pastaUploads, caminhoArquivo);
                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await model.Imagem.CopyToAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERRO UPLOAD IMAGEM: {ex.Message}");
                    TempData["ErroStatus"] = "Erro ao fazer upload da imagem.";
                }
            }

       
            var produtoTemporario = new ProdutoModel
            {
                Nome = model.Nome ?? string.Empty,
                Descricao = model.Descricao ?? string.Empty,
                Preco = model.Preco,
                CategoriaId = model.CategoriaId,
                Categoria = categoriaConvertida,
                Marca = model.Marca ?? string.Empty,
                Quantidade = model.Quantidade,
                Peso = model.Peso,
                Ativo = model.Ativo,
                ImagemUrl = caminhoArquivo,
                Sku_Tipo = model.Sku_Tipo,
                Sku_Agente = model.Sku_Agente,
                Sku_Capacidade = model.Sku_Capacidade,
                Sku_Modelo = model.Sku_Modelo,
                Sku = string.Empty
            };

            try
            {
                produtoTemporario.Sku = await _skuService.GerarSkuAsync(produtoTemporario);
            }
            catch (Exception ex)
            {
                TempData["ErroStatus"] = $"Erro ao gerar SKU: {ex.Message}";
                return View(model);
            }

            ProdutoBaseModel produtoFinal = ProdutoConverter.ConverterParaModelEspecifico(produtoTemporario);

            if (produtoFinal == null)
            {
                TempData["ErroStatus"] = "Erro ao converter para o modelo específico da categoria.";
                return View(model);
            }

            try
            {
                var produtoSalvo = await _produtoRepository.AddProdutoAsync(produtoFinal);

                TempData["SucessoFormProduto"] = $"Produto '{produtoSalvo.Nome}' adicionado! SKU: {produtoSalvo.Sku}";
                return RedirectToAction(nameof(GerenciarProdutos));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO SALVAR PRODUTO: {ex.Message}");
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData["ErroStatus"] = $"Erro ao salvar: {msg}";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AtualizarProduto(ProdutoBaseModel model)
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
                TempData["SucessoFormProduto"] = $"Produto '{model.Nome}' atualizado com sucesso.";
            }
            else
            {
                TempData["ErroStatus"] = "Erro ao atualizar produto. Verifique os campos obrigatórios.";
            }

            return RedirectToAction("GerenciarProdutos");
        }

        [HttpGet]
        public async Task<IActionResult> ListarProdutos()
        {
            try
            {
                var produtos = await _produtoRepository.GetAllProdutosAsync();
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
            var produto = await _produtoRepository.GetProdutoByIdAsync(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirProdutoHome(int id)
        {

            await _produtoRepository.RemoveProdutoAsync(id);
            TempData["SucessoFormProduto"] = "Produto excluído com sucesso.";
            return RedirectToAction("Index"); 
        }
    }
}