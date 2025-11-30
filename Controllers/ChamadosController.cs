using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Repository;
using ToughService.Services;
using System.Text;

namespace ToughService.Controllers
{
    public class ChamadosController : Controller
    {
        private readonly IChamadoRepository _chamadoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWhatsAppService _whatsAppService;
        private readonly ILogger<ChamadosController> _logger;

        public ChamadosController(
            IChamadoRepository chamadoRepository,
            UserManager<ApplicationUser> userManager,
            IWhatsAppService whatsAppService,
            ILogger<ChamadosController> logger)
        {
            _chamadoRepository = chamadoRepository;
            _userManager = userManager;
            _whatsAppService = whatsAppService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarServico(ChamadoModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.UserId = user.Id;
                }
            }


            model.DataSolicitacao = DateTime.Now;
            model.Status = StatusChamadoEnum.Novo; 

            if (model.Quantidade <= 0) model.Quantidade = 1;
            if (string.IsNullOrEmpty(model.TipoExtintor)) model.TipoExtintor = "A Combinar / Outros";

            if (ModelState.IsValid)
            {
                try
                {
                    await _chamadoRepository.AddChamadoAsync(model);

            
                    //  NOTIFICAÇÃO WHATSAPP (NOVO SERVIÇO)
               
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var msg = $"🛠️ *NOVA SOLICITAÇÃO DE SERVIÇO*\n" +
                                      $"--------------------------------\n" +
                                      $"📄 Protocolo: *#{model.Id}*\n" +
                                      $"👤 Cliente: *{model.NomeCliente}*\n" +
                                      $"📞 Telefone: {model.Telefone}\n" +
                                      $"🔧 Serviço: *{model.TipoServico}*\n" +
                                      $"📅 Data Desejada: {model.DataDesejada:dd/MM/yyyy}\n" +
                                      $"📍 Cidade: {model.Cidade} - {model.Bairro}\n" +
                                      $"📝 Obs: {model.Observacoes ?? "Nenhuma"}";

                            await _whatsAppService.EnviarMensagemAsync(msg);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Erro ao enviar WhatsApp de serviço: {ex.Message}");
                        }
                    });
                    // ==================================================================
                    TempData["SucessoForm"] = "Solicitação enviada com sucesso! Entraremos em contato.";
                    return RedirectToAction("Servicos", "Servicos");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao salvar chamado: {ex.Message}");
                    TempData["ErroForm"] = "Erro interno ao salvar. Tente novamente.";
                    return RedirectToAction("Servicos", "Servicos");
                }
            }

            // Se o modelo for inválido
            var erros = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            TempData["ErroForm"] = $"Verifique os dados: {erros}";
            return RedirectToAction("Servicos", "Servicos");
        }
    }
}