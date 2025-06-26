using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToughService.Models;
using ToughService.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ToughService.Controllers
{
    // Controlador responsável pelo registro, login e logout dos usuários
    public class RegistroController : Controller
    {
        // Contexto do banco de dados para acessar as tabelas
        private readonly BancoContext _context;

        // Construtor que recebe o contexto via injeção de dependência
        public RegistroController(BancoContext context)
        {
            _context = context;
        }

        // GET: Exibe o formulário para novo registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: Recebe os dados do formulário de registro e cria um novo usuário
        [HttpPost]
        public IActionResult Registro(RegistroModel registro)
        {
            // Verifica se os dados do formulário são válidos
            if (ModelState.IsValid)
            {
                // Checa se o email já está cadastrado no banco
                if (_context.Usuarios.Any(u => u.Email == registro.Email))
                {
                    // Adiciona erro de validação para email já existente
                    ModelState.AddModelError("Email", "Email já cadastrado.");
                    return View(registro);
                }

                // Cria um novo objeto usuário com os dados do formulário
                var usuario = new UsuarioModel
                {
                    Nome = registro.Nome,
                    CpfCnpj = registro.CpfCnpj,
                    Email = registro.Email,
                    Senha = registro.Senha // Atenção: em produção deve usar hash para senhas!
                };

                // Adiciona o usuário ao contexto e salva no banco
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Após o registro, redireciona para a página de login
                return RedirectToAction("Login");
            }

            // Se dados inválidos, retorna a mesma view com mensagens de erro
            return View(registro);
        }

        // GET: Exibe o formulário de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Recebe os dados do login e autentica o usuário
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            // Verifica se os dados do formulário são válidos
            if (!ModelState.IsValid)
                return View(login);

            // Busca um usuário no banco que tenha o email e senha informados
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.Senha == login.Senha);

            if (usuario != null)
            {
                // Se encontrado, salva o Id do usuário na sessão para manter a autenticação
                HttpContext.Session.SetInt32("UserId", usuario.Id);

                // Redireciona para a página de perfil do usuário autenticado
                return RedirectToAction("Perfil", "Perfil");
            }

            // Se não encontrar, adiciona erro genérico de autenticação falhada
            ModelState.AddModelError("", "Email ou senha inválidos.");

            // Retorna para a view de login com erro
            return View(login);
        }

        // POST: Logout do usuário, limpa a sessão e redireciona para login
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public IActionResult Logout()
        {
            // Limpa todos os dados da sessão, encerrando a autenticação
            HttpContext.Session.Clear();

            // Redireciona para a tela de login
            return RedirectToAction("Login", "Registro");
        }
    }
}
