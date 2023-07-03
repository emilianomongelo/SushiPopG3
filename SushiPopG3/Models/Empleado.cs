using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPopG3.Models
{
    [Table("T_EMPLEADO")]
    public class Empleado : Usuario
    {
        
        [Display(Name = "Número de Legajo")]   
        public int? Legajo { get; set; }

    }
}
