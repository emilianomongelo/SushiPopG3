using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SushiPopG3.Models;

[Table("T_PEDIDO")]
public class Pedido
{
    [Key]
    public int Id { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(50, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [MinLength(3, ErrorMessage = ErrorViewModel.CaracteresMin)]
    [Display(Name = "Número de Pedido")]
    public int? NroPedido { get; set; }


    [Display(Name = "Fecha y Hora de Pedido")]
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    public DateTime? FechaCompra { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(999999999, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [MinLength(1, ErrorMessage = ErrorViewModel.CaracteresMin)]
    public decimal? SubTotal { get; set; }


    [Display(Name = "Gasto de envío")]
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(999999999, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [MinLength(1, ErrorMessage = ErrorViewModel.CaracteresMin)]
    public decimal? GastoEnvio { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(999999999, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [MinLength(1, ErrorMessage = ErrorViewModel.CaracteresMin)]
    public decimal? Total { get; set; }
   

    public int? Estado { get; set; }

    /*RELACIONES*/
    public virtual Reclamo? Reclamo { get; set; }
    [ForeignKey("Reclamo")]

    public int CarritoId { get; set; }

    [ForeignKey("CarritoId")]
    public Carrito? Carrito { get; set; }


    [ForeignKey("DescuentoId")]
    public Descuento? Descuento{ get; set;} 
}
