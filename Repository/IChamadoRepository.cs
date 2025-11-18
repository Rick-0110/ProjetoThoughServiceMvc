using ToughService.Models;

namespace ToughService.Repository
{
    public interface IChamadoRepository
    {
     
        Task AddChamadoAsync(ChamadoModel chamado);

       
        Task<IEnumerable<ChamadoModel>> GetAllChamadosAsync();

     
        Task UpdateStatusChamadoAsync(int chamadoId, StatusChamadoEnum novoStatus);

        Task<List<ChamadoModel>> GetChamadosByUserIdAsync(string userId);

    }
}