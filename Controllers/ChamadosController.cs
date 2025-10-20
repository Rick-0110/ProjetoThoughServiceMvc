using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoThoughServiceMvc.Models;
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
        public async Task<IActionResult> SolicitarServico([FromForm] ChamadoModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErroForm"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dados inválidos.";
            
                return RedirectToAction("Servicos", "Servicos");
            }
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }
            model.UserId = user.Id;

            model.DataSolicitacao = DateTime.Now;
            model.Status = StatusChamadoEnum.Novo;
            model.TipoServico = Request.Form["TipoServico"];

            try
            {
                await _chamadoRepository.AddChamadoAsync(model);

               
                TempData["SucessoForm"] = "Sua solicitação foi enviada com sucesso! Entraremos em contato em breve.";
                return RedirectToAction("Servicos", "Servicos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar chamado: {ex}");
                TempData["ErroForm"] = "Ocorreu um erro interno ao processar sua solicitação.";
                return RedirectToAction("Servicos", "Servicos");
            }
        }
      
    }
}
