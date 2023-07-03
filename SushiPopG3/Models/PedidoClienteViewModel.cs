namespace SushiPopG3.Models
{
    public class PedidoClienteViewModel
    {

        public Pedido? Pedido { get; set; }

        public ICollection<CarritoItem>? CarritoItems { get; set; }

        public Cliente? Cliente { get; set; }





    }
}
