using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SushiPopG3.Models;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace SushiPopG3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbContext _context;

        public HomeController(ILogger<HomeController> logger, DbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var diaSemana = ObtenerNumeroDiaSemana();

            var infoDescuento = await _context.Descuento.Include(d => d.Producto).Where(d => d.Dia == diaSemana && d.Activo == true).FirstOrDefaultAsync();

            int descuento = 0;
            string producto = string.Empty;

            if (infoDescuento != null)
            {
                var descuentoProducto = await _context.Descuento
                    .Where(d => d.Dia == diaSemana && d.IdProducto == infoDescuento.IdProducto).FirstOrDefaultAsync();
                descuento = descuentoProducto.Porcentaje;
                producto = descuentoProducto.Producto.Nombre;

            }
                
            DescuentoDiaViewModel descuentoDiaViewModel = new DescuentoDiaViewModel();
            descuentoDiaViewModel.Dia = ObtenerNombreDiaHoy().ToUpper();
            descuentoDiaViewModel.Descuento = descuento;
            descuentoDiaViewModel.Producto = producto;

            return View(descuentoDiaViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static string ObtenerNombreDiaHoy()
        {
            CultureInfo culturaEspaniol = new CultureInfo("es-ES");
            return DateTime.Now.ToString("dddd", culturaEspaniol);
        }

        public static int ObtenerNumeroDiaSemana()
        {
            var nombreDia = DateTime.Now.DayOfWeek.ToString();
            switch (nombreDia)
            {
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                case "Sunday":
                    return 7;
            }
            return 0;

        }

        private static Descuento GenerarDescuento()
        {
            var descuento = new Descuento();
            return descuento;//Recorre la lista de productos y al precio le aplica el descuento que corresponde segun el dia

        }



        
    }
}