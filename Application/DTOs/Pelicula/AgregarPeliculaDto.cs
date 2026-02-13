using SistemaGestionPeliculas.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionPeliculas.Application.DTOs.Pelicula
{
    public class AgregarPeliculaDto

    {
        /// El título es obligatorio y no puede estar vacío.
        [Required(ErrorMessage = "El titulo es requerido")]
        public string Titulo { get; set; } = string.Empty;

        // La descripción es opcional, pero si se proporciona, no puede exceder los 500 caracteres.
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Descripcion { get; set; }

        // El precio es obligatorio y debe ser un valor decimal positivo.
        [Required(ErrorMessage = "El precio es requerido")]
        //Se estable 0.01 como mínimo para evitar que se ingresen precios negativos o cero.
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal Precio { get; set; }

        //la categoria es obligatoria
        [Required(ErrorMessage = "la categoria es requerido")]
        public int CategoriaId { get; set; }

        //Estado de la pelicula.
        public EstadoPelicula Estado { get; set; } = EstadoPelicula.Activo;

    }
}
