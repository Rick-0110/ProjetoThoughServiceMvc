using System.Threading.Tasks;
using ToughService.Models.Produtos; 

namespace ToughService.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailDestino, string assunto, string mensagemHtml);
    }
}