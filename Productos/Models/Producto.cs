﻿using System.ComponentModel.DataAnnotations;

namespace Productos.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Codigo_Producto { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public int? Fk_Categoria { get; set; }
        public string? Estado { get; set; }
        public string? Foto { get; set; }

        // Propiedad de navegación
        public virtual Categoria? Categoria { get; set; }
        public virtual Stock? Stock { get; set; }
    }
}

