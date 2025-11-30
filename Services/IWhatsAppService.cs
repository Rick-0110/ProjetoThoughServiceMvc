namespace ToughService.Services
{
    public interface IWhatsAppService
    {
        Task EnviarMensagemAsync(string mensagem);
    }
}
