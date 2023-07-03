using System;
using SushiPopG3.Models;

namespace SushiPopG3
{
    public class DataSeeder
    {
        public static void SeedData(DbContext dbContext)
        {
            // Agregar aquí la lógica para llenar la base de datos con datos de semilla
            // Por ejemplo:
            dbContext.Set<Categoria>().AddRange(
                new Categoria { Id = 1, Nombre = "Combos", Descripcion = "Combinación de productos" },
                new Categoria { Id = 2, Nombre = "Piezas", Descripcion = "Conjunto de piezas de sushi" },
                new Categoria { Id = 3, Nombre = "Salads", Descripcion = "Ensaladas para todos los gustos" },
                new Categoria { Id = 4, Nombre = "Entradas", Descripcion = "Entradas para compartir" },
                new Categoria { Id = 5, Nombre = "Salsas", Descripcion = "Salsas para condimentar tus comidas" },
                new Categoria { Id = 6, Nombre = "Bebidas", Descripcion = "Todo tipo de bebidas para acompañar" },
                new Categoria { Id = 7, Nombre = "Postres", Descripcion = "El broche de oro para cada comida" }
            );

            



            dbContext.SaveChanges();
        }
    }
}