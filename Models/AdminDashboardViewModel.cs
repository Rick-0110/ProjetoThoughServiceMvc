using System.Collections.Generic;

namespace ToughService.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalChamados { get; set; }
        public int TotalProdutosEmEstoque { get; set; }
        public int TotalPedidos { get; set; }

     
        public List<AdminActivityViewModel> AtividadesRecentes { get; set; } = new List<AdminActivityViewModel>();
    }
}