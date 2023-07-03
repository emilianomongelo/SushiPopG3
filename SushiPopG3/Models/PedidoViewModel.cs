namespace SushiPopG3.Models
{
    public class PedidoViewModel
    {
        public int CarritoId { get; set; }
        public List<CarritoItem> Productos { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }

        public decimal Subtotal { get; set; }

        public decimal GastoEnvio { get; set; }
    
    

    }
       
}
