namespace SushiPopG3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_CONTACTO")]
public class Contacto
{
    [Key]
	public int Id{ get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(255, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [Display(Name = "Correo electrónico")]
    [EmailAddress(ErrorMessage = ErrorViewModel.FormatoMail)]
    [MaxLength (100, ErrorMessage = ErrorViewModel.CaracteresMax)]
    public string Email { get; set; } 


    [Display(Name = "Teléfono")]
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [StringLength(10, ErrorMessage = ErrorViewModel.IgualCantidadCaracteres)]
    public string Telefono { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(4000, ErrorMessage = ErrorViewModel.CaracteresMax)]
    public string Mensaje { get; set; }


	public bool? Leido { get; set; } = false;


}
