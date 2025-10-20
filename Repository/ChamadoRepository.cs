using Microsoft.EntityFrameworkCore; 
using ProjetoThoughServiceMvc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Data;
using ToughService.Models;


namespace ToughService.Repository
{
    public class ChamadoRepository : IChamadoRepository
    {
        private readonly BancoContext _context;

        public ChamadoRepository(BancoContext context)
        {
            _context = context;
        }

        public async Task AddChamadoAsync(ChamadoModel chamado)
        {
            await _context.Chamados.AddAsync(chamado);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<ChamadoModel>> GetAllChamadosAsync()
        {
           return await _context.Chamados
                .Include(c => c.User)
                .OrderByDescending(c => c.DataSolicitacao)
                .ToListAsync();


        }

        public async Task<ChamadoModel> GetChamadoByIdAsync(int id)
        {
           
            return await _context.Chamados
                .Include(c => c.User) 
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateStatusChamadoAsync(int id, StatusChamadoEnum novoStatus)
        {
            var chamado = await _context.Chamados.FindAsync(id);
            if(chamado != null)
            {
                chamado.Status = novoStatus;
                _context.Chamados.Update(chamado);
                await _context.SaveChangesAsync();  

            }
        }
    }
}