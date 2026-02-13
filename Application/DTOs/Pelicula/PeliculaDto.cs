using SistemaGestionPeliculas.Domain.Enum;
using System.Text.Json.Serialization;

namespace SistemaGestionPeliculas.Application.DTOs.Pelicula
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioCop { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoPelicula Estado { get; set; }

    }

}