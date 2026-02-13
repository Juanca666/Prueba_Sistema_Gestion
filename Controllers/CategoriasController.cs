using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas.Infraestructure.Data;
using SistemaGestionPeliculas.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionPeliculas.Application.DTOs.Categoria;


namespace SistemaGestionPeliculas.Controllers;



[ApiController]
[Route("api/[controller]")]

public class CategoriasController : ControllerBase
{

    private readonly AppDbContext _context;
    private readonly ILogger<CategoriasController> _logger;

    // Constructor para inyectar el contexto de la base de datos y el logger
    public CategoriasController(AppDbContext context, ILogger<CategoriasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/categorias 
    // Obtener Todas las Categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        try
        {
            // Obtener todas las categorías de la base de datos y mapearlas a DTOs
            var categorias = await _context.Categorias
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion
                })
                .ToListAsync();

            return Ok(categorias);
        }
        //Manejo de errores
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las categorías");
            return StatusCode(500, new { error = "Error al obtener categorías" });
        }
    }

    //Categorias por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        try
        {
            // Buscar la categoría por ID en la base de datos
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound(new { error = $"Categoría con ID {id} no encontrada" });
            }
            // Mapear la entidad de categoría a un DTO
            var categoriaDto = new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion
            };

            return Ok(categoriaDto);
        }
        //Manejo de errores
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la categoría con ID {id}");
            return StatusCode(500, new { error = "Error al obtener categoría" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria(CrearCategoriaDto dto)
    {
        try
        {
            // Validar el modelo de entrada
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Crear la entidad de categoría a partir del DTO
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            //Logging de creación de categoría
            _logger.LogInformation("Categegoría creada con ID {Id} - {Nombre}", categoria.Id, categoria.Nombre);

            // Retornar el DTO de la categoría creada
            var categoriaDto = new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion
            };

            // Retornar la respuesta con el DTO de la categoría creada y la ubicación del recurso
            return CreatedAtAction(
              nameof(GetCategoria),
              new { id = categoria.Id },
              categoriaDto);
        }
        // Manejo de errores
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            return StatusCode(500, new { error = "Error al crear categoría" });
        }
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoria(int id, ActualizarCategoriaDto dto)
    {
        try
        {
            // Validar el modelo de entrada
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar la categoría por ID en la base de datos
            var categoria = await _context.Categorias.FindAsync(id);
            
            if (categoria == null)
            {
                return NotFound(new { error = $"Categoría con ID {id} no encontrada" });
            }

            // Actualizar los campos de la categoría con los valores del DTO
            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            // Logging de actualización de categoría
            _logger.LogInformation("Categoría actualizada con ID {Id} - {Nombre}", categoria.Id, categoria.Nombre);
            
            return NoContent();
        }
        // Manejo de errores
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, $"Error de actualización de la categoría con ID {id}");
            return StatusCode(409, new { error = "Error de actualización de categoría" });
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar la categoría con ID {id}");
            return StatusCode(500, new { error = "Error al actualizar categoría" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoria(int id)
    {
        try
        {
            // Buscar la categoría por ID en la base de datos
            var categoria = await _context.Categorias
                .Include(c => c.Peliculas) 
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (categoria == null)
            {
                return NotFound(new { error = $"Categoría con ID {id} no encontrada" });
            }

            if(categoria.Peliculas.Any())
            {
                return BadRequest(new { error = "No se puede eliminar la categoría porque tiene películas asociadas" });
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría eliminada con ID {Id} - {Nombre}", categoria.Id, categoria.Nombre);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar la categoría con ID {id}");
            return StatusCode(500, new { error = "Error al eliminar categoría" });
        }

    }

}