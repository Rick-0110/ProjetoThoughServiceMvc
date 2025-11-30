using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToughService.Models
{
    [Table("Pedidos")]
    public class PedidoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime DataPedido { get; set; }

        [Required]
        public StatusPedidoEnum Status { get; set; }

        [Required]
        [StringLength(200)]
        public string NomeCliente { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string EmailCliente { get; set; }

        [Required]
        [StringLength(20)]
        public string TelefoneCliente { get; set; }

        [Required]
        [StringLength(10)]
        public string Cep { get; set; }

        [Required]
        [StringLength(200)]
        public string Logradouro { get; set; }

        [Required]
        [StringLength(20)]
        public string Numero { get; set; }

        [StringLength(100)]
        public string? Complemento { get; set; }

        [Required]
        [StringLength(100)]
        public string Bairro { get; set; }

        [Required]
        [StringLength(100)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(2)]
        public string Estado { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoEnvio { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoPagamento { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustoEnvio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Desconto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        // Campos do Mercado Pago
        [StringLength(100)]
        public string? MercadoPagoPaymentId { get; set; }

        [StringLength(100)]
        public string? MercadoPagoPreferenceId { get; set; }

        [StringLength(50)]
        public string? MercadoPagoStatus { get; set; }

        public DateTime? MercadoPagoPaymentDate { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<PedidoItemModel> Itens { get; set; } = new List<PedidoItemModel>();
    }
}

