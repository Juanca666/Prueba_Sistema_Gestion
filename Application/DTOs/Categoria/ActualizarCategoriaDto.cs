using System.ComponentModel.DataAnnotations;

namespace SistemaGestionPeliculas.Application.DTOs.Categoria
{
    // Se Crea de esta manera para tener un mejor manejo de los datos y poder validar los datos que se reciben al actualizar una categoria.
    public class ActualizarCategoriaDto
    {
        //Validaciones Para el Nombre de la Categoria.
        [Required(ErrorMessage = "El nombre de la categoria es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la categoria no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;


        //Validacion para la Descripción de la Categoria.
        [StringLength(500,ErrorMessage = "La descrición de la categoria no puede exceder los 500 caracteres.")]
        public string? Descripcion { get; set; }
    }
}
