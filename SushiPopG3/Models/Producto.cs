using SushiPopG3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;


namespace SushiPopG3.Models 
{
    [Table("T_PRODUCTO")]
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMax)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(20, ErrorMessage = ErrorViewModel.CaracteresMin)]
        [MaxLength(250, ErrorMessage = ErrorViewModel.CaracteresMax)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Range(1, double.PositiveInfinity)]
        public decimal Precio { get; set; }

        [Display(Name = "URL Fotografía")]
        [MaxLength(1000, ErrorMessage = ErrorViewModel.CaracteresMax)]
        public string? Foto { get; set; }

        [Range(1, double.PositiveInfinity)]
        public int? Stock { get; set; }

        /*RELACIONES*/

        [ForeignKey("CategoriaId")]
        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        public ICollection<Descuento>? Descuentos { get; set; }

        public ICollection<CarritoItem>? CarritoItems { get; set; }

    }



}
