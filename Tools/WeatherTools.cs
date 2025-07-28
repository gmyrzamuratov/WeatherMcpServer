using System.ComponentModel;
using ModelContextProtocol.Server;
using WeatherMcpServer.Interfaces;

namespace WeatherMcpServer.Tools
{
    public class WeatherTools
    {
        private readonly IWeatherService _weatherService;

        public WeatherTools(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [McpServerTool]
        [Description("Get current weather conditions for a specified location/city.")]
        public async Task<string> GetCurrentWeather(
            [Description("Name of the city or location to get current weather for (e.g., 'London', 'New York', 'Tokyo')")] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return "Please provide a valid city or location name.";
            }

            return await _weatherService.GetCurrentWeatherAsync(location.Trim());
        }

        [McpServerTool]
        [Description("Get weather forecast for a specified location for the next few days.")]
        public async Task<string> GetWeatherForecast(
            [Description("Name of the city or location to get weather forecast for (e.g., 'London', 'New York', 'Tokyo')")] string location,
            [Description("Number of days to forecast (1-5 days, default is 3)")] int days = 3)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return "Please provide a valid city or location name.";
            }

            if (days < 1 || days > 5)
            {
                return "Number of days must be between 1 and 5.";
            }

            return await _weatherService.GetWeatherForecastAsync(location.Trim(), days);
        }

        [McpServerTool]
        [Description("Get weather alerts and warnings for a specified location.")]
        public async Task<string> GetWeatherAlerts(
            [Description("Name of the city or location to get weather alerts for (e.g., 'London', 'New York', 'Tokyo')")] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return "Please provide a valid city or location name.";
            }

            return await _weatherService.GetWeatherAlertsAsync(location.Trim());
        }

        [McpServerTool]
        [Description("Describes random weather in the provided city. (Legacy tool for testing)")]
        public string GetCityWeather(
            [Description("Name of the city to return weather for")] string city)
        {
            // Read the environment variable during tool execution.
            // Alternatively, this could be read during startup and passed via IOptions dependency injection
            var weather = Environment.GetEnvironmentVariable("WEATHER_CHOICES");
            if (string.IsNullOrWhiteSpace(weather))
            {
                weather = "balmy,rainy,stormy";
            }

            var weatherChoices = weather.Split(",");
            var selectedWeatherIndex = Random.Shared.Next(0, weatherChoices.Length);

            return $"The weather in {city} is {weatherChoices[selectedWeatherIndex]}.";
        }
    }
}