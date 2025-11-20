using System.Threading.Tasks;
using ToughService.Models;

namespace ToughService.Services
{
    public interface ICheckoutViewModelBuilder
    {
        Task<CheckoutViewModel> BuildAsync(CheckoutViewModel? model = null);
    }
}

