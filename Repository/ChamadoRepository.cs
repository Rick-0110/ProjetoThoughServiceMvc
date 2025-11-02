using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughService.Data;
using ToughService.Models;           
using ToughService.Repository;

namespace ToughService.Repository
{
    public class ChamadoRepository : IChamadoRepository
    {
        private readonly BancoContext _context; // <-- VERIFIQUE O NOME DA SUA CLASSE DBCONTEXT

        // Injeção de dependência do DbContext
        public ChamadoRepository(BancoContext context) // <-- VERIFIQUE O NOME AQUI TAMBÉM
        {
            _context = context;
        }

        /// <summary>
        /// Implementação do método para adicionar chamado
        /// </summary>
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

        /// <summary>
        /// Implementação para buscar todos os chamados para o painel ADM
        /// </summary>
        public async Task<IEnumerable<ChamadoModel>> GetAllChamadosAsync()
        {
            // Busca todos os chamados e usa .Include() para carregar
            // os dados do usuário (ApplicationUser) associado (para mostrar email, etc.)
            // O ADMController fará a filtragem e ordenação na memória.
            return await _context.Chamados
                                 .Include(c => c.User)
                                 .ToListAsync();
        }

        /// <summary>
        /// Implementação para atualizar o status do chamado pelo ADM
        /// </summary>
        public async Task UpdateStatusChamadoAsync(int chamadoId, StatusChamadoEnum novoStatus)
        {
            var chamado = await _context.Chamados.FindAsync(chamadoId);

            if (chamado == null)
            {
                // Se não encontrar, lança uma exceção para o controller tratar
                throw new KeyNotFoundException($"Chamado com ID {chamadoId} não foi encontrado.");
            }

            // Atualiza apenas o status
            chamado.Status = novoStatus;

            // Salva a mudança no banco
            await _context.SaveChangesAsync();
        }
    }
}