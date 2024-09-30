using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Productos.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Productos.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ventas/Create
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(Venta venta, string detalles)
        {
            if (ModelState.IsValid)
            {
                // Asigna el UsuarioId desde el usuario autenticado
                venta.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                venta.Fecha = DateTime.Now;

                _context.Ventas.Add(venta);
                _context.SaveChanges();

                var listaDetalles = JsonConvert.DeserializeObject<List<DetalleVenta>>(detalles);
                foreach (var detalle in listaDetalles)
                {
                    detalle.VentaId = venta.Id; // Asignar el Id de la venta al detalle
                                                
                    _context.DetalleVentas.Add(detalle);
                }

                _context.SaveChanges(); // Guardar los detalles
                TempData["VentaCreada"] = "La venta se ha registrado exitosamente.";
                return RedirectToAction("Crear");
            }
            return View(venta);
        }

    

    // GET: Ventas
    public IActionResult Index()
        {
            var ventas = _context.Ventas.ToList();
            return View(ventas);
        }

        // Método para obtener el precio por código de producto
        [HttpGet]
        public JsonResult GetPrecioPorCodigo(string codigo)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Codigo_Producto == codigo);
            if (producto != null)
            {
                return Json(new { precio = producto.Precio });
            }
            return Json(new { precio = 0 });
        }
    }
}
