using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SushiPopG3.Models;

    public class DbContext : IdentityDbContext
    {
        public DbContext (DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<SushiPopG3.Models.Carrito> Carrito { get; set; } = default!;

        public DbSet<SushiPopG3.Models.Categoria>? Categoria { get; set; }

        public DbSet<SushiPopG3.Models.Cliente>? Cliente { get; set; }

        public DbSet<SushiPopG3.Models.Contacto>? Contacto { get; set; }

        public DbSet<SushiPopG3.Models.Descuento>? Descuento { get; set; }

        public DbSet<SushiPopG3.Models.Empleado>? Empleado { get; set; }

        public DbSet<SushiPopG3.Models.Pedido>? Pedido { get; set; }

        public DbSet<SushiPopG3.Models.Producto>? Producto { get; set; }

        public DbSet<SushiPopG3.Models.Reclamo>? Reclamo { get; set; }

        public DbSet<SushiPopG3.Models.Reserva>? Reserva { get; set; }

        public DbSet<SushiPopG3.Models.CarritoItem>? CarritoItem { get; set; }

    }
