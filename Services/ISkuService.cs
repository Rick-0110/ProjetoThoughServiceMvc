using ToughService.Models;

namespace ToughService.Services
{
    public interface ISkuService
    {
        Task<string> GerarSkuAsync(ProdutoModel produto);
    }
}   
