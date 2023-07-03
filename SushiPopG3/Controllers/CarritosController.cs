using System;
using System.Collections.Generic;
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
    public class CarritosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carritos
        [Authorize(Roles = "CLIENTE")]
        [HttpGet("Carrito/Index")]
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();
            var dbContext = _context.Carrito
                .Include(i => i.CarritoItems)
                .ThenInclude(p => p.Producto)
                .Where(c => c.ClienteId == cliente.Id && c.Cancelado == false && c.Procesado == false && c.CarritoItems.All(ci => ci.Cantidad > 0))
                .ToListAsync();

            /*if(dbContext.Count() == 0)
            {
                TempData["error"] = "No hay productos en el carrito.";
                return RedirectToAction("Index", "Home");

            }
            */

            return View(dbContext);

        }

        // GET: Carritos/Details/5
        //[Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include( c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            /*var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }*/

            /*var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();
            if (cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }
            */

            //if (carrito == null || carrito.ClienteId != cliente.Id)
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Procesado,Cancelado,CliendeId")] Carrito carrito)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("MensajeError", "Home");
                }

                var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();
                if (cliente == null)
                {
                    return RedirectToAction("MensajeError", "Home");
                }

                carrito.ClienteId = cliente.Id;
                carrito.Procesado = false;
                carrito.Cancelado = false;
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Edit/5
       [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.FindAsync(id);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();
            if (cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            if (carrito == null || carrito.ClienteId != cliente.Id)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carrito == null)
            {
                return Problem("Entity set 'DbContext.Carrito'  is null.");
            }
            var carrito = await _context.Carrito.FindAsync(id);
            if (carrito != null)
            {
                _context.Carrito.Remove(carrito);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(int id)
        {
          return (_context.Carrito?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        /* private double ObtenerTotal(List<CarritoItem> listaItems)
         {
             double total = 0;

             foreach (var item in listaItems)
             {
                 //total += item.Subtotal;
             }

             return total;
         }*/

        public void Procesado(Carrito carrito)
        {
            if (carrito != null)
            {
                carrito.Procesado = true;
            }
        }

        public void Cancelado(Carrito carrito)
        {
            if (carrito != null)
            {
                this.Edit(carrito.Id, carrito);
            }
        }



    }
}
