using System;

namespace ToughService.Models
{
    public class AdminActivityViewModel
    {
        public string Tipo { get; set; } 
        public string Mensagem { get; set; }
        public DateTime Data { get; set; }
        public string IconeCss { get; set; }
        public string CorCss { get; set; } 
    }
}