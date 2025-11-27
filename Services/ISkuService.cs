using ToughService.Models.Produtos;

namespace ToughService.Services
{
    public interface ISkuService
    {
        Task<string> GerarSkuAsync(IProdutoBase produto);
    }
}   
