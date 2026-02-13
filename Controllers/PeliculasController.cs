using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas.Infraestructure.Data;
using SistemaGestionPeliculas.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionPeliculas.Application.DTOs.Pelicula;
using SistemaGestionPeliculas.Infraestructure.Services;

namespace SistemaGestionPeliculas.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PeliculasController : ControllerBase
{
    private readonly AppDbContext  _context;
    private readonly ILogger<PeliculasController> _logger;
    private readonly ILogService _logService;
    private readonly IExchangeRateService _exchangeRateService;

    // Constructor para inyectar el contexto de la base de datos y el logger
    public PeliculasController(AppDbContext context, ILogger<PeliculasController> logger, ILogService logService, IExchangeRateService exchangeRateService)
    {
        _context = context;
        _logger = logger;
        _logService = logService;
        _exchangeRateService = exchangeRateService;
    }

    //Api para obtener todas las peliculas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PeliculaDto>>> GetPeliculas()
    {
        try
        {
            var peliculas = await _context.Peliculas
                .Include(p => p.Categoria)
                .Select(p => new PeliculaDto
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    PrecioCop = 0,  
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria!.Nombre,
                    Estado = p.Estado
                })
                .ToListAsync();

            foreach (var pelicula in peliculas)
            {
                pelicula.PrecioCop = await _exchangeRateService.ConvertUsdToCopAsync(pelicula.Precio);
            }

            return Ok(peliculas);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las películas");
            return StatusCode(500, new {Error = "Error al obtener Peliculas" });
        }

         

    }

    //Obtener peliculas por Id
    [HttpGet("{id}")]
    public async Task<ActionResult<PeliculaDto>> GetPelicula(int id)
    {
        try
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula == null)
            {
                return NotFound(new { Error = "Pelicula no encontrada" });
            }

            var peliculaDto = new PeliculaDto
            {
                Id = pelicula.Id,
                Titulo = pelicula.Titulo,
                Descripcion = pelicula.Descripcion,
                Precio = pelicula.Precio,
                PrecioCop = await _exchangeRateService.ConvertUsdToCopAsync(pelicula.Precio),
                CategoriaId = pelicula.CategoriaId,
                CategoriaNombre = pelicula.Categoria!.Nombre,
                Estado = pelicula.Estado
            };

            return Ok(peliculaDto);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la película con ID {id}");
            return StatusCode(500, new { Error = "Error al obtener la película" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<PeliculaDto>> CreatePelicula(AgregarPeliculaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);

            if(!categoriaExiste)
            {
                return BadRequest(new { 
                    Error = "La categoría  no existe" });
            }

            var pelicula = new Pelicula
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                CategoriaId = dto.CategoriaId,
                Estado = dto.Estado

            };

            _context.Peliculas.Add(pelicula);
            await _context.SaveChangesAsync();

            _logService.LogPeliculas("Crear", pelicula.Id, pelicula.Precio, pelicula.CategoriaId);

            _logger.LogInformation("Película creada: {Id} - {Titulo}", pelicula.Id, pelicula.Titulo);

            var peliculaCreada = await _context.Peliculas
                   .Include(p => p.Categoria)
                   .FirstAsync(p => p.Id == pelicula.Id);

            // Convertir a DTO con precio en COP
            var peliculaDto = new PeliculaDto
            {
                Id = peliculaCreada.Id,
                Titulo = peliculaCreada.Titulo,
                Descripcion = peliculaCreada.Descripcion,
                Precio = peliculaCreada.Precio,
                PrecioCop = await _exchangeRateService.ConvertUsdToCopAsync(peliculaCreada.Precio),
                CategoriaId = peliculaCreada.CategoriaId,
                CategoriaNombre = peliculaCreada.Categoria?.Nombre,
                Estado = peliculaCreada.Estado
            };

            return CreatedAtAction(nameof(GetPelicula), new { id = pelicula.Id }, peliculaDto);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la película");
            return StatusCode(500, new { Error = "Error al crear la película" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePelicula(int id, ActualizarPeliculaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Buscar Pelicula 
            var pelicula = await _context.Peliculas.FindAsync(id);
            
            if (pelicula == null)
            {
                return NotFound(new { Error = "Película no encontrada" });
            }

            //Verificar si la categoría existe
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            
            if (!categoriaExiste)
            {
                return BadRequest(new { Error = "La categoría no existe" });
            }

            //Actualizamos los campos de la película
            pelicula.Titulo = dto.Titulo;
            pelicula.Descripcion = dto.Descripcion;
            pelicula.Precio = dto.Precio;
            pelicula.CategoriaId = dto.CategoriaId;
            pelicula.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            _logService.LogPeliculas("Actualizar", pelicula.Id, pelicula.Precio, pelicula.CategoriaId);

            _logger.LogInformation("Película actualizada: {Id} - {Titulo}", pelicula.Id, pelicula.Titulo);
            
            return NoContent();
        }

        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, $"Error de actualización de la película con ID {id}");
            return StatusCode(409, new { Error = "Error de base de datos al actualizar la película" });
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar la película con ID {id}");
            return StatusCode(500, new { Error = "Error al actualizar la película" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePelicula(int id)
    {
        try
        {
            var pelicula = await _context.Peliculas.FindAsync(id);

            if (pelicula == null)
            {
                return NotFound(new { Error = "Película no encontrada" });
            }
            //Eliminar Pelicula
            _context.Peliculas.Remove(pelicula);
            await _context.SaveChangesAsync();

            _logService.LogPeliculas("Eliminar", pelicula.Id, pelicula.Precio, pelicula.CategoriaId);

            _logger.LogInformation("Película eliminada: {Id} - {Titulo}", pelicula.Id, pelicula.Titulo);
           
            return NoContent();
        }
             
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar la película con ID {id}");
            return StatusCode(500, new { Error = "Error al eliminar la película" });
        }
    }

}
