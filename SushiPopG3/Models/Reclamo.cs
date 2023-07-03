using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPopG3.Models
{
    [Table("T_RECLAMO")]
    public class Reclamo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(255, ErrorMessage = ErrorViewModel.CaracteresMax)]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [EmailAddress(ErrorMessage = ErrorViewModel.FormatoMail)]
        public string Email { get; set; } 

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [StringLength(10, ErrorMessage = ErrorViewModel.IgualCantidadCaracteres)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Número de pedido")]
        public int NumeroPedido { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(50, ErrorMessage = ErrorViewModel.CaracteresMin)]
        [MaxLength(4000, ErrorMessage = ErrorViewModel.CaracteresMax)]
        [Display(Name = "Detalle")]
        public string DetalleReclamo { get; set; }

        public int PedidoId { get; set; }

       [ForeignKey("PedidoId")]
       public  Pedido? Pedido { get; set; }

    }
}
