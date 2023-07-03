using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SushiPopG3.Models;

[Table("T_CARRITO")]
public class Carrito
{
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [Key]
    public int Id { get; set; }
  

    public bool? Procesado { get; set;}


    public bool? Cancelado { get; set; }


    /*RELACIONES*/

   
    public int? ClienteId { get; set; }

    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }

    public virtual Pedido? Pedido { get; set; }

    public List<CarritoItem>? CarritoItems { get; set; }

    

}
