namespace ToughService.Services
{
    public interface ICaptchaService
    {
        Task<bool> VerifyCaptchaAsync(string token, CancellationToken cancellationToken);
    }
}
