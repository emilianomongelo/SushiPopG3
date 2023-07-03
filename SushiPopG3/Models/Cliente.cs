using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPopG3.Models
{
    [Table("T_CLIENTE")]
    public class Cliente : Usuario
    {

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(10, ErrorMessage = ErrorViewModel.CaracteresMax)]
        [MinLength(6, ErrorMessage = ErrorViewModel.CaracteresMin)]
        [Display (Name = "Número de cliente")]
        public int? NroCliente { get; set; } 

        /*RELACIONES*/
        public ICollection<Reserva>? Reservas { get; set; }

        public ICollection<Carrito>? Carritos { get; set; }

        
    }
}
