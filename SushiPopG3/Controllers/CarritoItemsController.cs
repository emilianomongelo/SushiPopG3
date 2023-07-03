using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPopG3.Models;

namespace SushiPopG3.Controllers
{
    public class CarritoItemsController : Controller
    {

        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoItemsController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [HttpGet("CarritoItems/Index")]
        public async Task<IActionResult> Index()
        {
            var carritoContext = _context.CarritoItem.Include(c => c.Carrito).Include(c => c.Producto);
            var lista = await _context.CarritoItem.Include(c => c.Carrito).Include(c => c.Producto).ToListAsync();

            foreach (var carritoItem in lista)
            {
                var producto = await _context.Producto.FindAsync(carritoItem.ProductoId);
                carritoItem.PrecioUnitarioConDescuento = producto.Precio;
              
            }
            return View(await carritoContext.ToListAsync());
        }

        [HttpGet("CarritoItems/Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.CarritoId == id);

            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        [Authorize(Roles = "CLIENTE")] //pendiente por completar
        [HttpGet("CarritoItems/Create/{productoId?}")]
        public async Task<IActionResult> Create(int? productoId)
        {
            if (productoId == null)
            {

            }

            var producto = await _context.Producto.FindAsync(productoId);

            if (producto == null)
            {

            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {

            }

            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            //antes de agregar un item hay que tener un carrito
            var carritoCliente = await _context.Carrito
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .Where(c => c.Cliente.Email.ToUpper() == user.NormalizedEmail && c.Procesado == false && c.Cancelado == false)
                .FirstOrDefaultAsync();

            if (carritoCliente == null)
            {
                Carrito carrito = new Carrito(); // Aqui solo crea el carrito
                carrito.Procesado = false;
                carrito.Cancelado = false;
                carrito.ClienteId = cliente.Id;
                await _context.SaveChangesAsync();

                carritoCliente = await _context.Carrito // Aqui buscar el carrito creado
                    .Include(c => c.Cliente)
                    .Include(c => c.CarritoItems)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
            }

            var precioProducto = producto.Precio;
            var dia = HomeController.ObtenerNumeroDiaSemana();
            var descuento = await _context.Descuento.
                Where(c => c.IdProducto == producto.Id && c.Activo == true && c.Dia == dia)
                .FirstOrDefaultAsync();

            if (descuento != null)
            {
                var descuentoAplicar = 1 - descuento.Porcentaje / 100;
                if (descuento.DescuentoMax <= descuentoAplicar)
                {
                    precioProducto = precioProducto * descuentoAplicar;
                }
                else { 
                    precioProducto -= descuento.DescuentoMax;
                }
            }

            var itemBuscado = await _context.CarritoItem
                .Where(c => c.CarritoId == carritoCliente.Id && c.ProductoId == producto.Id)
                .ToListAsync();

            if (itemBuscado == null)
            {
                CarritoItem itemEnCarrito = new CarritoItem();
                itemEnCarrito.PrecioUnitarioConDescuento = precioProducto;
                itemEnCarrito.Cantidad = 1;
                itemEnCarrito.CarritoId = carritoCliente.Id;
                itemEnCarrito.ProductoId = producto.Id;

                _context.Add(itemEnCarrito);
                await _context.SaveChangesAsync(); // Guardamos en la base de datos

            }
            else 
            {
                itemBuscado[0].Cantidad += 1;
                _context.Update(itemBuscado);
                await _context.SaveChangesAsync();
            }
            // Ir a la grilla de items
            return RedirectToAction(nameof(Index), "Carritos");
        }


        [HttpPost("CarritoItems/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoItemId,ProductoId,CarritoId,ValorUnitario,Cantidad,SubTotal")] CarritoItem carritoItem)
        {
            if (ModelState.IsValid)
            {
                var producto = await _context.Producto.FindAsync(carritoItem.ProductoId);
                carritoItem.PrecioUnitarioConDescuento = producto.Precio;
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "CarritoId", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Nombre", carritoItem.ProductoId);
            return View(carritoItem);
        }


        [HttpGet("CarritoItems/Edit/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "CarritoId", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Nombre", carritoItem.ProductoId);
            return View(carritoItem);
        }


        [HttpPost("CarritoItems/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarritoItemId,ProductoId,CarritoId,ValorUnitario,Cantidad,SubTotal")] CarritoItem carritoItem)
        {
            if (id != carritoItem.CarritoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "carritoId", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Nombre", carritoItem.ProductoId);
            return View(carritoItem);
        }


        [HttpGet("CarritoItems/Delete/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }


        [HttpPost("CarritoItems/Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarritoItem == null)
            {
                return Problem("Entity set 'CarritoContext.CarritoItem'  is null.");
            }
            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem != null)
            {
                _context.CarritoItem.Remove(carritoItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoItemExists(int id)
        {
            return _context.CarritoItem.Any(e => e.Id == id);
        }



        [HttpGet("CarritoItems/DeleteAll/{id?}")]
        public async Task<IActionResult> DeleteAll(Carrito carrito)
        {
            if (carrito == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync();
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }



        [HttpPost("CarritoItems/DeleteAll/{id?}"), ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllConfirmed()
        {
            IEnumerable<CarritoItem> itemsList = _context.CarritoItem.ToList();
            foreach (var Item in itemsList)
            {
                _context.CarritoItem.Remove(Item);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> CancelarItem()
        {
            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(c => c.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();
            var carritoBuscado = await _context.Carrito.Include(x => x.CarritoItems).Where(x => x.ClienteId == cliente.Id
            && x.Procesado == false && x.Cancelado == false).FirstOrDefaultAsync();

            foreach (var item in carritoBuscado.CarritoItems)
            {
                var producto = await _context.Producto.FindAsync(item.ProductoId);
                producto.Stock += item.Cantidad;
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }
            carritoBuscado.Cancelado = true;
            _context.Update(carritoBuscado);
            await _context.SaveChangesAsync();

            TempData["success"] = "El carrito fue cancelado con exito.";

            return RedirectToAction("Index", "Categorias");
        }
      
    }

 

}



/*GET CarritoItems
    [Authorize("CLIENTE")]
    public async Task<IActionResult> CreateOrEditItem(int productoId, int cantidad)
    {
        var user = await _userManager.GetUserAsync(User);
        var cliente = await _context.Cliente.Where(c => c.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

        if (cliente == null)
        {
            TempData["error"] = "El cliente logueado no existe";
            return RedirectToAction("Index", "Home");
        }

        var pedido = await _context.Pedido.Include(x => x.Carrito).ThenInclude(x => x.Cliente).Where(x => x.Carrito.Cliente.Id == cliente.Id && x.Estado == 1 ).FirstOrDefaultAsync();

        if (pedido == null)
        {
            TempData["error"] = "No existe un pedido para confirmar";
            return RedirectToAction("Index", "Home");
        }

        var cantidadPedidos = await _context.Pedido
            .Include(x => x.Carrito)
            .ThenInclude(x => x.Cliente)
            .Where(x => x.Carrito.Cliente.Id == cliente.Id && x.FechaCompra.Value.Date == DateTime.Now.Date)
            .ToListAsync();
        if (cantidadPedidos.Count() > 3)
        {
            TempData["error"] = "No se pueden relizar mas de 3 pedidos en un mismo dia";
            return RedirectToAction("Index", "Carritos");
        }

        var producto = await _context.Producto.FindAsync(productoId);
        if (producto.Stock < cantidad)
        {
            TempData["error"] = "No hay Stock";
            return RedirectToAction("Index", "Carritos");

        }

        var carrito = await _context.Carrito.Include(c => c.Cliente).Include(c => c.CarritoItems).Where(c => c.ClienteId == cliente.Id).FirstOrDefaultAsync();

        if (carrito == null)
        {
            carrito = new Carrito
            {
                ClienteId = cliente.Id,
                Procesado = false,
                Cancelado = false,
                CarritoItems = new List<CarritoItem>()
            };

            _context.Add(carrito);
            await _context.SaveChangesAsync();
        }

        var item = carrito.CarritoItems.Where(i => i.ProductoId == productoId).FirstOrDefault();

        //Zona de Descuento//
         if (item == null)
         {
             decimal precioConDescuento = producto.Precio;
            var diaSemana = HomeController.ObtenerNumeroDiaSemana();

            var descuentoBuscado = await _context.Descuento.Where(x => x.Dia == diaSemana && x.Activo == true && x.IdProducto == productoId).FirstOrDefaultAsync();
             if (descuentoBuscado != null)
             {
                 decimal auxPorcentaje = (decimal)descuentoBuscado.Porcentaje;
                 var descuentoNominal = producto.Precio * (auxPorcentaje / 100);
                 if (descuentoNominal <= descuentoBuscado.DescuentoMax)
                 {
                     precioConDescuento = producto.Precio - descuentoNominal;
                 }
                 else 
                 {
                     precioConDescuento = producto.Precio - descuentoBuscado.DescuentoMax;
                 }
             }


        var carritoBuscado = await _context.Carrito.Where(x => x.ClienteId == cliente.Id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        item = new CarritoItem
        {
            CarritoId = carritoBuscado.Id,
            ProductoId = producto.Id,
            Cantidad = cantidad,
            PrecioUnitarioConDescuento = precioConDescuento
        };

            _context.Add(item);
            await _context.SaveChangesAsync();

            if (cantidad < 0)
            {
                TempData["success"] = "El producto se quito con exito";
            }
            else 
            {
                TempData["success"] = "El producto se agrego con exito";
            }
        }
        else
        {
            if (cantidad == 0)
            {
                producto.Stock += item.Cantidad;
                _context.Update(producto);
                await _context.SaveChangesAsync();
                _context.Remove(item);
                await _context.SaveChangesAsync();
                TempData["success"] = "El producto se elimino con exito";
                return RedirectToAction("Index", "Carritos");
            }
            else
            {
                item.Cantidad += cantidad;
                _context.Update(item);
                await _context.SaveChangesAsync();

                if (cantidad < 0)
                {
                    TempData["success"] = "El producto se quito con exito";
                }
                else
                {
                    TempData["success"] = "El producto se agrego con exito";
                }
            }
        }

        producto.Stock -= cantidad;
        _context.Update(producto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), "Carritos"); 
    }
    */


