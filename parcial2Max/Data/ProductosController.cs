using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parcial2Max.Data;
using parcial2Max.Models;

namespace parcial2Max.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // 1 - Listar productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        // 2 - Crear producto
        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductoPorId), new { id = producto.Id }, producto);
        }

        // 3 - Editar producto
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarProducto(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 4 - Eliminar producto
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET por Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProductoPorId(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();
            return producto;
        }

        // 5 - Ordenar por precio
        [HttpGet("ordenar-por-precio")]
        public async Task<ActionResult<IEnumerable<Producto>>> OrdenarPorPrecio()
        {
            return await _context.Productos.OrderBy(p => p.Precio).ToListAsync();
        }

        // 6 - Buscar por nombre
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarPorNombre([FromQuery] string nombre)
        {
            return await _context.Productos
                .Where(p => p.Nombre.Contains(nombre))
                .ToListAsync();
        }

        // 7 - Listar productos por categoría
        [HttpGet("por-categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<Producto>>> ListarPorCategoria(int categoriaId)
        {
            return await _context.Productos
                .Where(p => p.CategoriaId == categoriaId)
                .ToListAsync();
        }

        // 8 - Listar productos por proveedor
        [HttpGet("por-proveedor/{proveedorId}")]
        public async Task<ActionResult<IEnumerable<Producto>>> ListarPorProveedor(int proveedorId)
        {
            return await _context.Productos
                .Where(p => p.ProveedorId == proveedorId)
                .ToListAsync();
        }
    }
}

