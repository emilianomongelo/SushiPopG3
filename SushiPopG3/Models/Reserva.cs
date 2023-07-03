using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPopG3.Models;

[Table("T_RESERVA")]
public class Reserva
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    [MaxLength(50, ErrorMessage = ErrorViewModel.CaracteresMax)]
    public string Local { get; set; }

    [Display(Name= "Fecha y Hora")]
    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    public DateTime FechaHora { get; set; }

    [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
    public bool? Confirmada { get; set; }

    public int IdCliente { get; set; }
    
    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }


}
