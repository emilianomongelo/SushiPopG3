using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPopG3.Models
{
    [Table("T_CARRITOITEM")]
    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }

        public decimal? PrecioUnitarioConDescuento { get;  set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Range(1, double.PositiveInfinity)]
        public int Cantidad { get; set; }

        /*RELACIONES*/
      
        public int? CarritoId { get; set; }

        [ForeignKey("CarritoId")]
        public Carrito? Carrito {get; set;}

        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

    

    }
}
