using Microsoft.AspNetCore.Mvc;
using ToughService.Data;
using ToughService.Models;
using Microsoft.AspNetCore.Http;

// Controlador responsável pela exibição e atualização do perfil do usuário
public class PerfilController : Controller
{
    // Contexto do banco de dados para acessar as entidades
    private readonly BancoContext _context;

    // Construtor recebe o contexto via injeção de dependência
    public PerfilController(BancoContext context)
    {
        _context = context;
    }

    // Ação GET para exibir o perfil do usuário
    [HttpGet]
    public IActionResult Perfil()
    {
        // Obtém o Id do usuário logado da sessão
        int? userId = HttpContext.Session.GetInt32("UserId");

        // Se não estiver logado, redireciona para a página de login
        if (userId == null)
            return RedirectToAction("Login", "Registro");

        // Busca o usuário no banco pelo Id
        var usuario = _context.Usuarios.Find(userId.Value);

        // Se não encontrar o usuário, também redireciona para login
        if (usuario == null)
            return RedirectToAction("Login", "Registro");

        // Envia o usuário para a view para exibir os dados do perfil
        return View(usuario);
    }

    // Ação POST para atualizar os dados do perfil
    [HttpPost]
    [ValidateAntiForgeryToken] // Previne ataques CSRF
    public IActionResult AtualizarPerfil(UsuarioModel model)
    {
        // Obtém o Id do usuário logado da sessão
        int? userId = HttpContext.Session.GetInt32("UserId");

        // Se não estiver logado, redireciona para login
        if (userId == null)
            return RedirectToAction("Login", "Registro");

        // Busca o usuário no banco pelo Id
        var usuario = _context.Usuarios.Find(userId.Value);

        // Se não encontrar o usuário, redireciona para login
        if (usuario == null)
            return RedirectToAction("Login", "Registro");

        // Atualiza os campos que o usuário pode alterar no perfil
        usuario.Nome = model.Nome;
        usuario.CpfCnpj = model.CpfCnpj;

        // Salva as alterações no banco de dados
        _context.SaveChanges();

        // Define uma mensagem temporária para informar sucesso na atualização
        TempData["Mensagem"] = "Perfil atualizado com sucesso!";

        // Redireciona para a página do perfil, mostrando as informações atualizadas
        return RedirectToAction("Perfil");
    }
}
