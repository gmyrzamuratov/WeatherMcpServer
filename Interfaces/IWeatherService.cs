namespace WeatherMcpServer.Interfaces
{
    public interface IWeatherService
    {
        Task<string> GetCurrentWeatherAsync(string location);
        Task<string> GetWeatherForecastAsync(string location, int days = 3);
        Task<string> GetWeatherAlertsAsync(string location);
    }
}
