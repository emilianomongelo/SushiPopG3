using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPopG3.Models;

namespace SushiPopG3.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly DbContext _context;

        public CategoriasController(DbContext context)
        {
            _context = context;
        }

        // GET: Categorias
        
        public async Task<IActionResult> Index()
        {
            //Incluimos los productos asociados a esa categoria
             var categorias = await _context.Categoria.Include(c => c.Productos).ToListAsync();
            return View(categorias);
        }

        // GET: Categoris/Details/5
        [HttpGet("Categorias/Details/{nombre?}")]
        public async Task<IActionResult> Details(string? nombre)
        {
            if (nombre == null || _context.Categoria == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categoria.Where(m => m.Nombre.Equals(nombre)).FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound();
            }

            return View("Details",categoria);
        }

        // GET: Categoris/Create
        [Authorize(Roles = "ADMIN")]
        public IActionResult Create()
        {
      
            return View();
        }

        // POST: Categoris/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Categorias/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            var category = await _context.Categoria.Where(c => c.Nombre.Equals(categoria.Nombre)).FirstOrDefaultAsync();

            if (category != null)
            {
                ModelState.AddModelError("Nombre", "Nombre de categoria repetido");
                return View(categoria);
            }

            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categoris/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categoris/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
            return View(categoria);
        }

        // GET: Categoris/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categoria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categoris/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categoria == null)
            {
                return Problem("Entity set 'DbContext.Categoria'  is null.");
            }
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria != null)
            {
                _context.Categoria.Remove(categoria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
          return (_context.Categoria?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
