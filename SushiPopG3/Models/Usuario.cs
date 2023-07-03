namespace SushiPopG3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class Usuario

{
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMin)]
    [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMax)]
    [Key]
    public int Id { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMin)]
    [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMax)]
    public string Nombre { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMin)]
    [MaxLength(90, ErrorMessage = ErrorViewModel.CaracteresMax)]
    public string Apellido { get; set; }
  

    [Display(Name = "Direccion")]
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMax)]
    public string Direccion { get; set; }


    [Display(Name = "Telefono")]
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [StringLength(10, ErrorMessage = ErrorViewModel.IgualCantidadCaracteres)]
	public string Telefono { get; set; }


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [Display(Name = "Fecha de Nacimiento")]
	public DateTime FechaDeNacimiento { get; set; }


    [Display(Name = "Fecha de Alta")]
    [DataType(DataType.Date)]
    public DateTime? FechaAlta { get; set; }



    public bool? Activo { get; set; } = true;


    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [EmailAddress(ErrorMessage = ErrorViewModel.FormatoMail)]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; }

}
