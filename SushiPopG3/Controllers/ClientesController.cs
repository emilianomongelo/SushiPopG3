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
    public class ClientesController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientesController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clientes
        [Authorize(Roles = "ADMIN, EMPLEADO")]
        [HttpGet("Clientes/Index")]
        public async Task<IActionResult> Index()
        {
            var listaClientes = await _context.Cliente.ToListAsync(); 
            return View(listaClientes);
        }

        // GET: Clientes/Details/5
        [Authorize(Roles = "ADMIN, EMPLEADO")]
        [HttpGet("Clientes/Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            return View(cliente);
        }


        [Authorize(Roles = "CLIENTE")]
        [HttpGet("Clientes/Details")]
        public async Task<IActionResult> PersonalDetail()
        {
            var user = await _userManager.GetUserAsync(User); // Obtengo usuario logueado

            if (user == null)
            {
                return RedirectToAction("MensajeError", "Home"); // Si es nulo voy a la pantalla de error
            }

            if (_context.Cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            // Busco un cliente por el email normalizado
            var cliente = await _context.Cliente.Where(c => c.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            if (cliente == null)
            {
                /*
                En este punto existe un usuario logeado pero no un cliente. 
                Esto significa que el usuario se registro pero abandonó la pantalla de carga de datos del cliente.

                Para terminar el registro mandamos a la vista con el formulario para que termine de completarlo.
                 */
                return RedirectToAction("Create", "Clientes", user);
            }

            return View("Details", cliente); // Reutilizo la vista Details
        }

        // GET: Clientes/Create
         public IActionResult Create(IdentityUser? user)
         {
            if (user == null) return RedirectToAction("MensajeError", "Home");
            //Crea un cliente y le asigna el mismo email que se hizo durante el registro de identity
            Cliente cliente = new Cliente()
            {
                Email = user.Email
            };
             return View(cliente);
         }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Direccion,Telefono,FechaDeNacimiento,Email")] Cliente cliente)
        {
            // Verifico que no Existe un cliente con el mismo mail
            if (await existeMail(cliente.Email))
            {
                ModelState.AddModelError("Email", "El Email ya existe");
                return View(cliente);
            }

            if (ModelState.IsValid)
            {
                //Si no esta duplicado llamamos al metodo asignar numero de cliente para generar el numero
                cliente.NroCliente = await AsignarNroCliente();

                //Seteamos la fecha de alta
                cliente.FechaAlta = DateTime.Now;

                //Asignamos el flag de cliente activo
                cliente.Activo = true;

                //Guardamos el ciente en el contexto
                _context.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [HttpGet("Clientes/Edit/{id}")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            var user = await _userManager.GetUserAsync(User); // Obtengo usuario logueado
            if (user == null)
            {
                return RedirectToAction("MensajeError", "Home"); // Si es nulo va a la pantalla de error
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            if (cliente.Email.ToUpper() != user.NormalizedEmail) // Si el email del cliente no coincide con el normalizado va a la pantalla de error
            {
                return RedirectToAction("MensajeError", "Home");
            }

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost("Clientes/Edit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int id, [Bind("NroCliente,Id,Nombre,Apellido,Direccion,Telefono,FechaDeNacimiento,FechaAlta,Activo,Email")] Cliente cliente)
        {
            var user = await _userManager.GetUserAsync(User); // obtengo el usuario logeado

            if (id != cliente.Id || cliente.Email.ToUpper() != user.NormalizedEmail) // agrego la validación de usuario
            {
                return RedirectToAction("MensajeError", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return RedirectToAction("MensajeError", "Home");
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        [HttpGet("Clientes/Delete/{id}")]
        [Authorize(Roles = "ADMIN, EMPLEADO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente == null)
            {
                return RedirectToAction("MensajeError", "Home");
            }

            return View(cliente);
        }


        // POST: Clientes/Delete/5
        [Authorize(Roles = "ADMIN, EMPLEADO")]
        [HttpPost("Clientes/Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cliente == null)
            {
                return Problem("Entity set 'DbContext.Cliente'  is null.");
            }

            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente != null)
            {
                // Borro el usuario
                var user = await _userManager.FindByEmailAsync(cliente.Email);
                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();

                // Borro el cliente
                _context.Cliente.Remove(cliente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.Id == id);
        }

        //Metodo privado para validar si el mail esta repetido:
        private async Task<bool> existeMail(string email)
        {
            var cliente = await _context.Cliente.Where(c => c.Email == email).FirstOrDefaultAsync();
            return cliente != null;
        }

        //Metodo privado para generar el numero de cliente:
        private async Task<int> AsignarNroCliente()
        {
            var cliente = await _context.Cliente.OrderByDescending(e => e.NroCliente).FirstOrDefaultAsync();

            return (int)((cliente == null) ? 4200000 : cliente.NroCliente + 1);

        }



    }
}
