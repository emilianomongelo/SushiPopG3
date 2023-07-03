using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;  
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SushiPopG3.Models;

namespace SushiPopG3.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PedidosController(DbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> AllPedidos()
        {
            return _context.Pedido != null ?
                View(await _context.Pedido.ToListAsync()) :
                Problem("DbContext.Pedido is null");
        }

        [Authorize(Roles = "CLIENTE, ADMIN")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            Cliente cliente = await _context.Cliente.FindAsync(user.Id); // aqui hay un error revisarlo con where en vez de Find
            var pedidos = await _context.Pedido.Where(p => p.Carrito.Cliente.Id.Equals(user.Id)).ToListAsync();

            if (pedidos.Count != 0)
            {
                List<PedidoClienteViewModel> listaPedidos = new List<PedidoClienteViewModel>();

                foreach (var pedido in pedidos)
                {
                    PedidoClienteViewModel pedidosClienteViewMdel = new PedidoClienteViewModel();
                    pedidosClienteViewMdel.Pedido = pedido;
                    pedidosClienteViewMdel.CarritoItems =
                        await _context.CarritoItem.Where(x => x.CarritoId == pedido.CarritoId).ToListAsync();
                    pedidosClienteViewMdel.Cliente = cliente;
                    listaPedidos.Add(pedidosClienteViewMdel);
                }

                return View(listaPedidos);
            }
            return RedirectToAction("Index", "Home");
            /*var pedido = await _context.Pedido
                .Include(x => x.Carrito)
                .Include(x => x.Carrito.Cliente)
                .Include(x => x.Carrito.CarritoItems)
                .ThenInclude(x => x.Producto)
                .ToListAsync();

            if (User.IsInRole("CLIENTE"))
            {
                var user = await _userManager.GetUserAsync(User);
                var pedidosCliente = pedido.Where(x => x.Carrito.Cliente.Email.ToUpper() == user.NormalizedEmail
                && x.FechaCompra >= DateTime.Now.AddDays(-90)).ToList();

                if (pedidosCliente == null)
                {
                    TempData["error"] = "No se econtraron pedidos.";
                    return RedirectToAction("Index", "Home");
                }
                return View(pedidosCliente);

            }

            var pedidosEmpleado = pedido.Where(x => (x.Estado != 5 || x.Estado != 6)).ToList();

            if (pedidosEmpleado == null)
            {
                TempData["error"] = "No se encontraron pedidos.";
                return RedirectToAction("Index", "Home");
            }

            return View(pedidosEmpleado);*/
        }


        // GET: Pedidos/Details/5
        [Authorize(Roles = "CLIENTE, ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empleado == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }


        // GET: Pedidos/Details/5
        [Authorize(Roles = "CLIENTE, ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        // GET: Pedidos/Details/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE, ADMIN")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NroPedido,FechaCompra,Subtotal,Descuento,GastoEnvio,Total,Estado")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }


        public async Task<IActionResult> SeguirPedido()
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("ADMIN") || User.IsInRole("EMPLEADO"))
                {
                    TempData["error"]= "Administradores y empleados no pueden hacer pedidos.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    var pedidoCliente = await _context.Pedido.Include(x => x.Carrito).ThenInclude(c => c.Cliente)
                        .Where(x => x.Carrito.Cliente.Email.ToUpper() == user.NormalizedEmail)
                        .OrderByDescending(x => x.NroPedido).FirstOrDefaultAsync();

                    PedidoEstadoViewModel pedidoVm = new PedidoEstadoViewModel()
                    {
                        NroPedido = (int)pedidoCliente.NroPedido
                    };
                    return await SeguirPedido(pedidoVm);

                }
            }


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SeguirPedido([Bind("NroPedido")] PedidoEstadoViewModel pedido)
        {
            var estadoPedido = await _context.Pedido.Where(x => x.NroPedido == pedido.NroPedido)
                .FirstOrDefaultAsync();
            if (estadoPedido == null)
            {
                TempData["error"] = "No se encontró el numero de pedido";
            }
            return View(pedido);
        }

        public string EstadoPedido(Pedido pedido)
        {
            switch (pedido.Estado)
            {
                case 1: return "Sin confirmar";
                case 2: return "Confirmado";
                case 3: return "En Preparacion";
                case 4: return "En reparto";
                case 5: return "Entregado";
                case 6: return "Cancelado";
            }
            return string.Empty;
        }

        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Cancelar(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                TempData["error"] = "No se encontro el Id";
                return RedirectToAction("Index", "Pedidos");
            }

            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                TempData["error"] = "No se encontro el pedido.";
                return RedirectToAction("Index", "Pedidos");

            }

            pedido.Estado = 6;
            _context.Update(pedido);
            await _context.SaveChangesAsync();
            TempData["success"] = "Se canceló el pedido";
            return RedirectToAction("Index", "Pedidos");

        }

        [Authorize(Roles = "CLIENTE")]

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var carrito = await _context.Carrito.Include(x => x.Cliente).Include(x => x.CarritoItems)
                .ThenInclude(x => x.Producto).Where(x => x.Cliente.Email.ToUpper()==user.NormalizedEmail)
                .FirstOrDefaultAsync();

            decimal gastoEnvio = 80;
            string apiKey = " ";
            string nombreCiudad = "Buenos Aires";
            string apiUrl = $" PAGINA DE LA API DEL CLIMA ?q={nombreCiudad}&appid={apiKey}";
            using (HttpClient cliente = new HttpClient())
            {
                HttpResponseMessage resp = await cliente.GetAsync(apiUrl);
                if (resp.IsSuccessStatusCode)
                {

                    if (/* ESTA LLOVIENDO*/ false)
                    {
                        gastoEnvio = 120;
                    }
                }
            }

            var cantidadPedidos = await _context.Pedido.Include(x => x.Carrito).ThenInclude(x => x.Cliente)
                .Where(x => x.Carrito.Cliente.Email.ToUpper() == user.NormalizedEmail
                && x.Estado == 5 && x.FechaCompra >= DateTime.Now.AddDays(-30)).ToListAsync();

            if (cantidadPedidos.Count()>=10)
            {
                gastoEnvio = 0;
            }

            PedidoViewModel pedidoViewModel = new PedidoViewModel()
            {
                CarritoId = carrito.Id,
                Productos = carrito.CarritoItems.ToList(),
                Cliente = carrito.Cliente.Nombre + " " + carrito.Cliente.Apellido,
                Direccion = carrito.Cliente.Direccion,
                Subtotal = (decimal)carrito.CarritoItems.Sum(x => x.PrecioUnitarioConDescuento * x.Cantidad),
                GastoEnvio = gastoEnvio
            };
            return View(pedidoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoId,Subtotal,GastoEnvio")] PedidoViewModel pedidoVm)
        {
            if (ModelState.IsValid)
            {
                Pedido pedido = new Pedido();
                pedido.CarritoId = pedidoVm.CarritoId;
                pedido.NroPedido = await ObtenerNroPedido();
                pedido.FechaCompra = DateTime.Now;
                pedido.SubTotal = pedidoVm.Subtotal;
                pedido.GastoEnvio = pedidoVm.GastoEnvio;
                pedido.Total = pedidoVm.Subtotal + pedidoVm.GastoEnvio;
                pedido.Estado = 1;

                _context.Add(pedido);
                await _context.SaveChangesAsync();

                var carrito = await _context.Carrito.FindAsync(pedidoVm.CarritoId);
                carrito.Procesado = true; _context.Update(carrito);
                await _context.SaveChangesAsync();

                TempData["success"]= "El pedido se creó con exito.";
                return RedirectToAction(nameof(Index));

            }


            return View(pedidoVm);
        }

       /* [Authorize(Roles = "EMPLEADO")]

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null || _context.Pedido == null)
            {
                TempData["error"] = "no se encontró el Id.";
                return RedirectToAction("Index", "Pedidos");
            }
            var pedido = await _context.Pedido.FindAsync(id);
            if(pedido == null)
            {
                TempData["error"]="No se encontró el pedido";
                return RedirectToAction("ndex", "Pedidos");

            }

            return View(pedido);
        }
       */


       /* [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id, NroPedido,FechaCompra,Subtotal,GastoEnvio,Total,Estado,CarritoId")] Pedido pedido)
        {
            if (id!=pedido.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var pedidoBuscado = await _context.Pedido.FindAsync(pedido.Id);
                    if(pedido.Estado < pedidoBuscado.Estado)
                    {
                        TempData["error"] = "No puede cambiarse a un estado anterior.";
                        return View(pedido);
                    }
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"]= "EL pedido se actualizó con exito.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"]= "El pedido no se pudo actualizar";
            return View(pedido);

        }*/

        private bool PedidoExists(int id)
        {
            return (_context.Pedido?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<int> ObtenerNroPedido()
        {
            var pedido = await _context.Pedido.OrderByDescending(x => x.NroPedido).FirstOrDefaultAsync();
            if(pedido == null)
            {
                return 30000;
            }
            else
            {
                return (int)(pedido.NroPedido + 1);
            }
        }
         



    }

}




            ////////////////////////////////////////////////////////////////////


        /*



            var dbContext = _context.Pedido.Include(c => c.Carrito).Include(c => c.Carrito.CarritoItems);
            var lista = await _context.CarritoItem.Include(c => c.Producto).ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            var carrito = await _context.Carrito
                .Include(x => x.Cliente)
                .Include(x => x.CarritoItems)
                .ThenInclude(x => x.Producto)
                .Where(x => x.Cliente.Email.ToUpper()==user.NormalizedEmail)
                .FirstOrDefaultAsync();

            foreach (var producto in lista)
            {
                /*var producto = await _context.Producto.FindAsync(productoId);
                carritoItem.PrecioUnitarioConDescuento = producto.Precio;
                carritoItem.SubTotal = carritoItem.Cantidad * carritoItem.PrecioUnitarioConDescuento; PREGUNTAR?
            */
        /*
        }
            return View(await dbContext.ToListAsync());
       
    }

        // GET: Pedidos/Details/5
        [HttpGet("Pedido/Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(c => c.Carrito)
                .Include(c => c.Carrito.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
        /*
        // GET: Pedidos/Create
        [HttpGet("Pedido/Create")]
        [Authorize(Roles = "CLIENTE")]
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Pedido, "Id", "Id");
            ViewData["ClienteId"] = new SelectList(_context.Producto, "Id", "Apellido");
            return View();
        }
        */
        /*
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var carrito = await _context.Carrito
                .Include(x => x.Cliente)
                .Include(x => x.CarritoItems)
                .ThenInclude(x => x.Producto)
                .Where(x => x.Cliente.Email.ToUpper()==user.NormalizedEmail)
                .FirstOrDefaultAsync();
            decimal gastoEnvio = 80;
            string apiKey = "e77bf38b94ed30b69bddf7c944f0be49";
            


            return View();
        }*/

        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  /*      [HttpPost("Pedido/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NroPedido,FechaCompra,SubTotal,Descuento,GastoEnvio,Total,Estado")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                var carritoItem = await _context.CarritoItem.FindAsync(pedido.CarritoId);
                carritoItem.PrecioUnitarioConDescuento = carritoItem.Producto.Precio;
                pedido.SubTotal = carritoItem.Cantidad * carritoItem.PrecioUnitarioConDescuento;
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            //ViewData["ProductoId"] = new SelectList(_context.Producto, "Id", "Descripcion", pedido.Carrito.CarritoItems.id);
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        [HttpGet("Pedido/Edit/{id?}")]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            //ViewData["ProductoId"] = new SelectList(_context.Producto, "Id", "Descripcion", pedido.Carrito.CarritoItems.id);
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Pedido/Edit/{id?}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NroPedido,FechaCompra,SubTotal,Descuento,GastoEnvio,Total,Estado")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            //ViewData["ProductoId"] = new SelectList(_context.Producto, "Id", "Descripcion", pedido.Carrito.CarritoItems.id);
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        [HttpGet("Pedido/Delete/{id?}")]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(c => c.Carrito)
                .Include(c => c.Carrito.CarritoItems)
                .FirstOrDefaultAsync();
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost("Pedido/Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedido == null)
            {
                return Problem("Entity set 'DbContext.Pedido'  is null.");
            }
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedido.Remove(pedido);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
          return (_context.Pedido?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("Pedidos/DeleteAll/{id?}")]
        public async Task<IActionResult> DeleteAll(Carrito carrito)
        {
            if (carrito == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(c => c.Carrito)
                .Include(c => c.Carrito.CarritoItems)
                .FirstOrDefaultAsync();
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        [HttpPost("CarritoItems/DeleteAll/{id?}"), ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllConfirmed()
        {
            IEnumerable<Pedido> itemsList = _context.Pedido.ToList();
            foreach (var Item in itemsList)
            {
                _context.Pedido.Remove(Item);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
  */