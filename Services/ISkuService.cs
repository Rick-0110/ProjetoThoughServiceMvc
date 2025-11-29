using System.Threading.Tasks;
using ToughService.Models.Produtos; 

namespace ToughService.Services
{
    public interface ISkuService
    {
        Task<string> GerarSkuAsync(ProdutoBaseModel produto);
    }
}