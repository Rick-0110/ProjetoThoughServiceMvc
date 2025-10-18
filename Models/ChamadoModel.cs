using Microsoft.AspNetCore.Http;
using ProjetoThoughServiceMvc.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToughService.Models;
using ToughService.Data;

public class ChamadoModel
{
    public int Id { get; set; } 

 
    [Required] public string NomeCliente { get; set; }
    public DateTime DataSolicitacao { get; set; }
    [Required] public DateTime DataDesejada { get; set; }
    [Required] public string TipoServico { get; set; }
    [Required] public string TipoExtintor { get; set; }
    [Required][Range(1, int.MaxValue)] public int Quantidade { get; set; }
    [Required] public string Cep { get; set; }
    [Required] public string Logradouro { get; set; }
    [Required] public string Numero { get; set; }
    [Required] public string Bairro { get; set; }
    [Required] public string Cidade { get; set; }
    [Required] public string Estado { get; set; }
    public string Complemento { get; set; }
    [Required] public string Telefone { get; set; }
    public string Observacoes { get; set; }
    public StatusChamadoEnum Status { get; set; }


    // --- ADIÇÕES PARA VINCULAR AO USUÁRIO ---
    [Required] // Torna obrigatório que um chamado pertença a um usuário
    public string UserId { get; set; } // Chave Estrangeira para AspNetUsers

    [ForeignKey("UserId")] // Vincula explicitamente à propriedade UserId
    public virtual ApplicationUser User { get; set; } // Propriedade de navegação (EF Core)
                                                      // --- FIM DAS ADIÇÕES ---
}