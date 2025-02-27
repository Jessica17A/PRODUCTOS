﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Agregar este using
using Microsoft.EntityFrameworkCore;
using Productos.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser> // Cambia a IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetalleVentas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Llama a base primero

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany() // Ajusta esta relación según tu caso
            .HasForeignKey(p => p.Fk_Categoria);

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Stock)
            .WithOne(s => s.Producto)
            .HasForeignKey<Stock>(s => s.Fk_Producto);
    }
}
