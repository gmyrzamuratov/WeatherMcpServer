using WeatherMcpServer.Models;

namespace WeatherMcpServer.Interfaces
{
    public interface IWeatherRepository
    {
        Task<WeatherResponse?> GetCurrentWeatherAsync(string location);
        Task<ForecastResponse?> GetWeatherForecastAsync(string location, int days = 5);
        Task<WeatherAlertsResponse?> GetWeatherAlertsAsync(double lat, double lon);
    }
}
