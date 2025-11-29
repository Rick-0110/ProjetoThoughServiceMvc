using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;          
using ToughService.Repository;    

namespace ToughService.Controllers
{
    public class ChamadosController : Controller
    {
        private readonly IChamadoRepository _chamadoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChamadosController(IChamadoRepository chamadoRepository, UserManager<ApplicationUser> userManager)
        {
            _chamadoRepository = chamadoRepository;
            _userManager = userManager;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken] 
        [Authorize]
        public async Task<IActionResult> SolicitarServico(ChamadoModel model)
        {
            
            if (!ModelState.IsValid)
            {
                
                string errorMsg = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dados inválidos. Verifique o formulário.";
                TempData["ErroForm"] = errorMsg;
              
                return RedirectToAction("Servicos", "Servicos");
            }

           
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErroForm"] = "Erro: Usuário não autenticado.";
                return RedirectToAction("Servicos", "Servicos");
            }

          
            model.UserId = user.Id; 
            model.DataSolicitacao = DateTime.Now;
            model.Status = StatusChamadoEnum.Novo;

           
            try
            {
                await _chamadoRepository.AddChamadoAsync(model);

            
                TempData["SucessoForm"] = "Sua solicitação foi enviada com sucesso! Entraremos em contato em breve.";
                return RedirectToAction("Servicos", "Servicos");
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"ERRO AO SALVAR CHAMADO: {ex.Message}");
                TempData["ErroForm"] = "Ocorreu um erro interno ao processar sua solicitação. Tente novamente.";
                return RedirectToAction("Servicos", "Servicos");
            }
        }
    }
}