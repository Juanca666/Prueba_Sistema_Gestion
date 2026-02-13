namespace SistemaGestionPeliculas.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ? Descripcion { get; set; }

        public ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>(); // Relación con Pelicula

    }
}
