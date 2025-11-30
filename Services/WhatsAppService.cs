using System.Net;

namespace ToughService.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly HttpClient _httpClient;

        public WhatsAppService(IConfiguration configuration, ILogger<WhatsAppService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task EnviarMensagemAsync(string mensagem)
        {
            try
            {
                var phone = _configuration["WhatsAppSettings:PhoneNumber"];
                var apikey = _configuration["WhatsAppSettings:ApiKey"];

                if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(apikey))
                {
                    _logger.LogWarning("WhatsApp não configurado no appsettings (PhoneNumber ou ApiKey vazios).");
                    return;
                }
                string mensagemCodificada = WebUtility.UrlEncode(mensagem);

                string url = $"https://api.callmebot.com/whatsapp.php?phone={phone}&text={mensagemCodificada}&apikey={apikey}";

                // Envia a requisição (Dispara a mensagem)
                await _httpClient.GetAsync(url);

                _logger.LogInformation("Notificação de WhatsApp enviada para a empresa com sucesso.");
            }
            catch (Exception ex)
            {
                // Apenas loga o erro, não trava o checkout do cliente se o zap falhar
                _logger.LogError($"Erro ao enviar WhatsApp: {ex.Message}");
            }
        }
    }
}