using System.Data;

namespace SistemaGestionPeliculas.Infraestructure.Services;

public interface IExchangeRateService
{
    //Convertir de USD a COP
    Task<decimal> ConvertUsdToCopAsync(decimal usdAmount);
    //Define tasas de datos actuales
    Task<ExchangeRateResponse> GetExchangeRatesAsync();
}

//Respuesta Api Externa Tasa de cambio.
public class ExchangeRateResponse
{
    public decimal UsdRate { get; set; }
    public decimal CopRate { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ExchangeRateService : IExchangeRateService
{ 
    //Datos privados para el servicio de tasa de cambio.
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRateService> _logger;
    private ExchangeRateResponse ? _cachedRates;
    private DateTime _cacheExpiration = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    //Constructor del servicio de tasa de cambio, inyectando HttpClient y Logger.
    public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    //Obtener Datos de cambio con cache
    public async Task<ExchangeRateResponse> GetExchangeRatesAsync()
    {
        if(_cachedRates != null && DateTime.Now < _cacheExpiration)
        {
            _logger.LogInformation("usando tasas de cambio cache");
            return _cachedRates;
        }
        try
        {
            _logger.LogInformation("obteniendo tasas de cambio de la API externa");
            var rates =  new ExchangeRateResponse
            {
                UsdRate = 1.0m,
                CopRate = 3683m,
                LastUpdated = DateTime.Now
            };

            _cachedRates = rates;
            _cacheExpiration = DateTime.Now.Add(_cacheDuration);

            return rates;
        }

        //Manejo de Errores en caso de falla de la API externa, se devuelve tasas por defecto.
        catch (HttpRequestException ex)
        {   
            _logger.LogError(ex, "Error al obtener tasas de cambio de la API externa");

            return new ExchangeRateResponse

            {
                UsdRate = 1.0m,
                CopRate = 3683m,
                LastUpdated = DateTime.Now
            };
        }

    }

    //Convertir USD A COP
    public async Task<decimal> ConvertUsdToCopAsync(decimal usdAmount)
    {
        var rates = await GetExchangeRatesAsync();
        return usdAmount * rates.CopRate;
    }


}
