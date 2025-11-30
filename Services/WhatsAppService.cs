using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account; 
namespace ToughService.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;

        public WhatsAppService(IConfiguration configuration, ILogger<WhatsAppService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];

            if (!string.IsNullOrEmpty(accountSid) && !string.IsNullOrEmpty(authToken))
            {
                try
                {
                    TwilioClient.Init(accountSid, authToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao iniciar Twilio: {ex.Message}");
                }
            }
        }

        public async Task EnviarMensagemAsync(string mensagem)
        {
            try
            {
                var fromNumber = _configuration["Twilio:FromNumber"];
                var toNumber = _configuration["Twilio:MyPhoneNumber"];

                // Validação de segurança
                if (string.IsNullOrEmpty(fromNumber) || string.IsNullOrEmpty(toNumber))
                {
                    _logger.LogWarning("Números do Twilio não configurados.");
                    return;
                }

                var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{toNumber}"));
                messageOptions.From = new PhoneNumber($"whatsapp:{fromNumber}");
                messageOptions.Body = mensagem;

                await MessageResource.CreateAsync(messageOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar WhatsApp Twilio: {ex.Message}");
            }
        }
    }
}