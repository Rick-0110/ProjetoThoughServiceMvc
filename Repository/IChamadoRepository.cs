


using ToughService.Models;

namespace ToughService.Repository
{
    public interface IChamadoRepository
    {
        Task AddChamadoAsync(ChamadoModel chamado); 
        Task<IEnumerable<ChamadoModel>> GetAllChamadosAsync();
        Task<ChamadoModel> GetChamadoByIdAsync(int id);
        Task UpdateStatusChamadoAsync(int id, StatusChamadoEnum novoStatus);
    }
}
