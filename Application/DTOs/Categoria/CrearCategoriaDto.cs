using System.ComponentModel.DataAnnotations;

namespace SistemaGestionPeliculas.Application.DTOs.Categoria
{
    //Dto Para Crear una Categoria.
    public class CrearCategoriaDto
    {
        //Validaciones Para el Nombre de la Categoria.
        [Required(ErrorMessage = "El nombre de la categoria es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la categoria no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;


        //Validaciones Para la Descripción de la Categoria.
        [StringLength(500, ErrorMessage = "La descripción de la categoria no puede exceder los 500 caracteres.")]
        public string? Descripcion { get; set; }
    }
}
