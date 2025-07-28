using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherMcpServer.Interfaces;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Repositories
{
    public class OpenWeatherMapRepository : IWeatherRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenWeatherMapRepository> _logger;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

        public OpenWeatherMapRepository(HttpClient httpClient, ILogger<OpenWeatherMapRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            // Get API key from environment variable
            _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? string.Empty;
            
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OpenWeatherMap API key not found in environment variable OPENWEATHER_API_KEY");
            }
        }

        public async Task<WeatherResponse?> GetCurrentWeatherAsync(string location)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogError("API key is not configured");
                    return null;
                }

                var url = $"{BaseUrl}/weather?q={Uri.EscapeDataString(location)}&appid={_apiKey}&units=metric";
                _logger.LogInformation("Fetching current weather for location: {Location}", location);

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch weather data. Status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(jsonContent);
                
                _logger.LogInformation("Successfully fetched current weather for {Location}", location);
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current weather for location: {Location}", location);
                return null;
            }
        }

        public async Task<ForecastResponse?> GetWeatherForecastAsync(string location, int days = 5)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogError("API key is not configured");
                    return null;
                }

                // OpenWeatherMap 5-day forecast API returns data in 3-hour intervals
                // We'll limit the results based on the days parameter
                var url = $"{BaseUrl}/forecast?q={Uri.EscapeDataString(location)}&appid={_apiKey}&units=metric";
                _logger.LogInformation("Fetching {Days}-day forecast for location: {Location}", days, location);

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch forecast data. Status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var forecastData = JsonSerializer.Deserialize<ForecastResponse>(jsonContent);
                
                // Filter forecast data to requested number of days
                if (forecastData?.List != null && days < 5)
                {
                    var cutoffTime = DateTimeOffset.UtcNow.AddDays(days).ToUnixTimeSeconds();
                    forecastData.List = forecastData.List.Where(item => item.Dt <= cutoffTime).ToList();
                    forecastData.Cnt = forecastData.List.Count;
                }
                
                _logger.LogInformation("Successfully fetched {Days}-day forecast for {Location}", days, location);
                return forecastData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching forecast for location: {Location}", location);
                return null;
            }
        }

        public async Task<WeatherAlertsResponse?> GetWeatherAlertsAsync(double lat, double lon)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogError("API key is not configured");
                    return null;
                }

                // Note: Weather alerts are part of OpenWeatherMap's One Call API 3.0
                // which requires a different endpoint and may have different pricing
                var url = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&appid={_apiKey}&exclude=minutely,hourly,daily";
                _logger.LogInformation("Fetching weather alerts for coordinates: {Lat}, {Lon}", lat, lon);

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch weather alerts. Status: {StatusCode}. This may be due to API plan limitations.", response.StatusCode);
                    // Return empty alerts response instead of null to indicate no alerts rather than an error
                    return new WeatherAlertsResponse { Alerts = new List<WeatherAlert>() };
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                
                // Parse the One Call API response and extract alerts
                using var document = JsonDocument.Parse(jsonContent);
                var alerts = new List<WeatherAlert>();
                
                if (document.RootElement.TryGetProperty("alerts", out var alertsElement))
                {
                    foreach (var alertElement in alertsElement.EnumerateArray())
                    {
                        var alert = JsonSerializer.Deserialize<WeatherAlert>(alertElement.GetRawText());
                        if (alert != null)
                        {
                            alerts.Add(alert);
                        }
                    }
                }
                
                _logger.LogInformation("Successfully fetched {AlertCount} weather alerts for coordinates: {Lat}, {Lon}", alerts.Count, lat, lon);
                return new WeatherAlertsResponse { Alerts = alerts };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather alerts for coordinates: {Lat}, {Lon}", lat, lon);
                return new WeatherAlertsResponse { Alerts = new List<WeatherAlert>() };
            }
        }
    }
}
