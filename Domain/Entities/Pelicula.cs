using SistemaGestionPeliculas.Domain.Enum;

namespace SistemaGestionPeliculas.Domain.Entities
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CategoriaId { get; set; }
        public EstadoPelicula Estado { get; set; } = EstadoPelicula.Activo;

        public Categoria? Categoria { get; set; } // Relación con Categoria


    }
}
