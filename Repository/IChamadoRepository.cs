using ToughService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToughService.Repository
{
    public interface IChamadoRepository
    {
     
        Task AddChamadoAsync(ChamadoModel chamado);

       
        Task<IEnumerable<ChamadoModel>> GetAllChamadosAsync();

     
        Task UpdateStatusChamadoAsync(int chamadoId, StatusChamadoEnum novoStatus);

       
    }
}