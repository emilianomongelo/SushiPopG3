using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPopG3.Models;

[Table("T_CATEGORIA")]
public class Categoria
{
    [Key]
    public int Id{ get; set; }


	[Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
	[MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMax)]
	public string Nombre { get; set; }


	[Required(ErrorMessage = ErrorViewModel.CaracteresMax)]
	[MaxLength(1000, ErrorMessage = ErrorViewModel.CaracteresMax)]
	[MinLength(1, ErrorMessage = ErrorViewModel.CaracteresMin)]
	[Display(Name = "Descripción")]
	public string? Descripcion { get; set;}


	/*RELACIONES*/
	public ICollection<Producto>? Productos { get; set;}

}
