using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPopG3.Models;

namespace SushiPopG3.Controllers
{
    public class ReclamosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReclamosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reclamoes
        [Authorize(Roles = "ADMIN, EMPLEADO")]
        public async Task<IActionResult> Index()
        {
              return _context.Reclamo != null ? 
                          View(await _context.Reclamo.ToListAsync()) :
                          Problem("Entity set 'DbContext.Reclamo'  is null.");
        }

        // GET: Reclamoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reclamo == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reclamo == null)
            {
                return NotFound();
            }

            return View(reclamo);
        }

        // GET: Reclamoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reclamoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Reclamos/Create")]
        [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Precio,Foto,Stock")Reclamo reclamo)
        {
            // Obtenemos el Id actual
            string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtenenms el objeto usuario del iD actual
            IdentityUser user = _userManager.FindByIdAsync(idUser).Result;

            if (user != null)
            {
                //Buscamos un cliente exitente con el emil del User
                Cliente clienteLogueado = await _context.Cliente.Where(c => c.Email == user.Email).FirstOrDefaultAsync();
                if (clienteLogueado != null)
                {
                    Reclamo reclamo = new()
                    {
                        Email = clienteLogueado.Email,
                        NombreCompleto = clienteLogueado.Nombre + "  " + clienteLogueado.Apellido,
                        Telefono = clienteLogueado.Telefono
                    };
                    return View(reclamo);
                }
            }

            Reclamo reclamoVacio = new();
            return View(reclamoVacio);
        }
    }*/

        // GET: Reclamoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reclamo == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo.FindAsync(id);
            if (reclamo == null)
            {
                return NotFound();
            }
            return View(reclamo);
        }

        // POST: Reclamoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Email,Telefono,NumeroPedido,DetalleReclamo")] Reclamo reclamo)
        {
            if (id != reclamo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reclamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReclamoExists(reclamo.Id))
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
            return View(reclamo);
        }

        // GET: Reclamoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reclamo == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reclamo == null)
            {
                return NotFound();
            }

            return View(reclamo);
        }

        // POST: Reclamoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reclamo == null)
            {
                return Problem("Entity set 'DbContext.Reclamo'  is null.");
            }
            var reclamo = await _context.Reclamo.FindAsync(id);
            if (reclamo != null)
            {
                _context.Reclamo.Remove(reclamo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReclamoExists(int id)
        {
          return (_context.Reclamo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
