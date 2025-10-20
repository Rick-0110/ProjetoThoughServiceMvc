using Microsoft.AspNetCore.Authorization;
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
        [Authorize] // Garante que o usuário está logado antes de tentar salvar
        public async Task<IActionResult> SolicitarServico(ChamadoModel model)
        {
            // Verifica se os dados recebidos são válidos de acordo com as anotações no ChamadoModel
            if (!ModelState.IsValid)
            {
                // Pega a primeira mensagem de erro para exibir ao usuário
                string errorMsg = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dados inválidos. Verifique o formulário.";
                TempData["ErroForm"] = errorMsg;
                // Redireciona de volta para a página de Serviços para exibir o erro
                return RedirectToAction("Servicos", "Servicos");
            }

            // Se os dados são válidos, busca o usuário logado
            var user = await _userManager.GetUserAsync(User);
            // Teoricamente, user não será null por causa do [Authorize], mas é uma segurança extra
            if (user == null)
            {
                TempData["ErroForm"] = "Erro: Usuário não autenticado.";
                return RedirectToAction("Servicos", "Servicos");
            }

            // Preenche os campos que não vêm diretamente do formulário
            model.UserId = user.Id;
            model.DataSolicitacao = DateTime.Now;
            model.Status = StatusChamadoEnum.Novo;
            // model.TipoServico já deve ter vindo do campo oculto preenchido pelo JS

            // Tenta salvar no banco de dados
            try
            {
                await _chamadoRepository.AddChamadoAsync(model);
                // Define a mensagem de sucesso
                TempData["SucessoForm"] = "Sua solicitação foi enviada com sucesso! Entraremos em contato em breve.";
                // Redireciona de volta para a página de Serviços
                return RedirectToAction("Servicos", "Servicos");
            }
            catch (Exception ex)
            {
                // Em caso de erro ao salvar (problema no banco, etc.)
                // Idealmente, logar o erro 'ex' para diagnóstico
                Console.WriteLine($"ERRO AO SALVAR CHAMADO: {ex.Message}"); // Log simples
                TempData["ErroForm"] = "Ocorreu um erro interno ao processar sua solicitação. Tente novamente.";
                // Redireciona de volta para a página de Serviços
                return RedirectToAction("Servicos", "Servicos");
            }
        }
    }
}
