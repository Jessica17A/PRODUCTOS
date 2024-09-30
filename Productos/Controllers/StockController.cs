using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Productos.Models;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet; // Para Cloudinary

namespace Productos.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Cloudinary _cloudinary; // Añadir Cloudinary

        public StockController(ApplicationDbContext context, Cloudinary cloudinary) // Inyectar Cloudinary
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Stock
        public async Task<IActionResult> Index()
        {
            // Obtener productos que no tienen stock registrado
            var productosSinStock = await _context.Productos
                .Where(p => !_context.Stocks.Any(s => s.Fk_Producto == p.ProductoID))
                .ToListAsync();

            // Obtener productos con stock registrado
            var productosConStock = await _context.Productos
                .Where(p => _context.Stocks.Any(s => s.Fk_Producto == p.ProductoID))
                .ToListAsync();

            // Generar URL de la imagen para productos sin stock
            foreach (var producto in productosSinStock)
            {
                producto.Foto = ObtenerUrlImagen(producto.Foto); // Genera la URL segura
            }

            // Generar URL de la imagen para productos con stock
            foreach (var producto in productosConStock)
            {
                producto.Foto = ObtenerUrlImagen(producto.Foto); // Genera la URL segura
            }

            // Obtener stock para cada producto
            var stockDictionary = await _context.Stocks
                .ToDictionaryAsync(s => s.Fk_Producto);

            // Pasar datos a la vista
            ViewData["ProductosSinStock"] = productosSinStock;
            ViewData["ProductosConStock"] = productosConStock;
            ViewData["StockDictionary"] = stockDictionary;

            return View();
        }

        // Método para generar la URL segura de la imagen usando el public_id
        private string ObtenerUrlImagen(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return "/path/to/default/image.jpg"; // Devuelve una imagen predeterminada si no hay imagen
            }

            var url = _cloudinary.Api.UrlImgUp
                        .Secure(true) // Para generar una URL segura (https)
                        .BuildUrl(publicId);
            return url;
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarStock(int id, int nuevoStock)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.Fk_Producto == id);

            if (stock == null)
            {
                stock = new Stock
                {
                    Fk_Producto = id,
                    StockInicial = nuevoStock,
                    StockActual = nuevoStock,
                    StockMinimo = 5, // Asigna un valor predeterminado
                    StockMaximo = 30, // Asigna un valor predeterminado
                    TipoMovimiento = "Inicial"
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                stock.StockInicial = nuevoStock;
                stock.StockActual = nuevoStock;
                _context.Stocks.Update(stock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
