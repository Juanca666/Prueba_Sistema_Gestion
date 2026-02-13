using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas.Domain.Entities;
using SistemaGestionPeliculas.Domain.Enum;
namespace SistemaGestionPeliculas.Infraestructure.Data;



// Contexto de base de datos para la aplicación
public class AppDbContext : DbContext
{
    // Constructor que recibe las opciones de configuración del contexto
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    // DbSets para las entidades
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Pelicula> Peliculas { get; set; }

    // Configuración del modelo utilizando Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entity =>
        {
            // Definir Primary Key
            entity.HasKey(e => e.Id);

            // Configurar columna Nombre
            entity.Property(e => e.Nombre)
                .IsRequired()          
                .HasMaxLength(100);     

            // Configurar columna Descripcion
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500);     

            // Configurar relación con Peliculas
            entity.HasMany(e => e.Peliculas)        
                  .WithOne(p => p.Categoria)        
                  .HasForeignKey(p => p.CategoriaId) 
                  .OnDelete(DeleteBehavior.Restrict);

            //Sed Data Inicial
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Comedia", Descripcion = "Selección exclusiva para peliculas de comedia." },
                new Categoria { Id = 2, Nombre = "Drama", Descripcion = "Selección exclusiva para peliculas de drama." }
            );


        });


        modelBuilder.Entity<Pelicula>(entity =>
        {
            // Primary Key
            entity.HasKey(e => e.Id);

            // Columna Titulo
            entity.Property(e => e.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            // Columna Descripcion
            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000);

            // Columna Precio
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(18,2)"); 


            // Columna Estado (Enum)
            entity.Property(e => e.Estado)
                .HasConversion<string>();

            // Sed Data Inicial
            modelBuilder.Entity<Pelicula>().HasData(
                new Pelicula { Id = 1, Titulo = "Supercool", Descripcion = "Dos mejores amigos viven una noche de caos", Precio = 11.20m, Estado = EstadoPelicula.Activo, CategoriaId = 1 },
                new Pelicula { Id = 2, Titulo = "Son Como Niños", Descripcion = "Un grupo de amigos y excompañeros descubren que envejecer no siempre significa madurar", Precio = 20.00m, Estado = EstadoPelicula.Activo, CategoriaId = 1 },
                new Pelicula { Id = 3, Titulo = "El Secreto de sus Ojos", Descripcion = "Un oficial de justicia retirado decide escribir una novela sobre un caso de violación y asesinato ocurrido décadas atrás", Precio = 30.10m, Estado = EstadoPelicula.Activo, CategoriaId = 2 }
            );

        });

    }


    

}



