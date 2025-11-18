using Microsoft.EntityFrameworkCore;
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
            try
            {
                // Adiciona o objeto ao contexto
                await _context.Chamados.AddAsync(chamado);

                // Salva as mudanças no banco (foi aqui que depuramos)
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Loga o erro no console (para depuração)
                Console.WriteLine($"Erro ao salvar chamado no repositório: {ex.Message}");
                // Relança a exceção para que o Controller possa pegá-la no 'catch'
                throw;
            }
        }

  
        public async Task<IEnumerable<ChamadoModel>> GetAllChamadosAsync()
        {
            return await _context.Chamados
                                 .Include(c => c.User)
                                 .ToListAsync();
        }

        public async Task UpdateStatusChamadoAsync(int chamadoId, StatusChamadoEnum novoStatus)
        {
            var chamado = await _context.Chamados.FindAsync(chamadoId);

            if (chamado == null)
            {
                throw new KeyNotFoundException($"Chamado com ID {chamadoId} não foi encontrado.");
            }
            chamado.Status = novoStatus;

            await _context.SaveChangesAsync();
        }


        public async Task<List<ChamadoModel>> GetChamadosByUserIdAsync(string userId)
        {
            return await _context.Chamados
                                 .Where(c => c.UserId == userId)
                                 .OrderByDescending(c => c.DataSolicitacao)
                                 .ToListAsync();
        }
    }
}