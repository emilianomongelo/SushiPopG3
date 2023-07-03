namespace SushiPopG3.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

[Table("T_DESCUENTO")]
public class Descuento
{
    [Key]
    public int Id { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [Display(Name = "Día")]
    [Range(1,7)]
    public int Dia { get; set; } 


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [Range(1, 100)]
    public int Porcentaje { get; set; } 


    [Display(Name = "Descuento máximo")]
    [Range(1, double.PositiveInfinity)]
    public decimal DescuentoMax { get; set; }

 
    public bool? Activo { get; set; }

    /*RELACIONES*/

    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(50, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [MinLength(3, ErrorMessage = ErrorViewModel.CaracteresMin)]
    [ForeignKey("ProductoId")]
    public int? IdProducto { get; set; }
  

    public Producto? Producto { get; set; }

}
 