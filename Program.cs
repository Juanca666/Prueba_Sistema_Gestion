using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas.Infraestructure.Data;
using SistemaGestionPeliculas.Infraestructure.Services;

var builder = WebApplication.CreateBuilder(args);

//Conexion a la Bd
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});


// Inyeccion de dependencias
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();

// Controlladores
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Migrar la base de datos al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        Console.WriteLine("Base de datos migrada exitosamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al migrar BD: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.UseDefaultFiles(); 

app.UseStaticFiles();   

app.Run();
