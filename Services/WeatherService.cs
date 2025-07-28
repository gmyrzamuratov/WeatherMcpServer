using Microsoft.Extensions.Logging;
using System.Text;
using WeatherMcpServer.Interfaces;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IWeatherRepository weatherRepository, ILogger<WeatherService> logger)
        {
            _weatherRepository = weatherRepository;
            _logger = logger;
        }

        public async Task<string> GetCurrentWeatherAsync(string location)
        {
            try
            {
                var weatherData = await _weatherRepository.GetCurrentWeatherAsync(location);
                
                if (weatherData == null)
                {
                    return $"Unable to retrieve weather data for '{location}'. Please check the location name and try again.";
                }

                return FormatCurrentWeather(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current weather for location: {Location}", location);
                return $"An error occurred while retrieving weather data for '{location}'.";
            }
        }

        public async Task<string> GetWeatherForecastAsync(string location, int days = 3)
        {
            try
            {
                // Limit days to reasonable range
                days = Math.Max(1, Math.Min(5, days));
                
                var forecastData = await _weatherRepository.GetWeatherForecastAsync(location, days);
                
                if (forecastData == null)
                {
                    return $"Unable to retrieve forecast data for '{location}'. Please check the location name and try again.";
                }

                return FormatWeatherForecast(forecastData, days);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather forecast for location: {Location}", location);
                return $"An error occurred while retrieving forecast data for '{location}'.";
            }
        }

        public async Task<string> GetWeatherAlertsAsync(string location)
        {
            try
            {
                // First get current weather to obtain coordinates
                var weatherData = await _weatherRepository.GetCurrentWeatherAsync(location);
                
                if (weatherData?.Coord == null)
                {
                    return $"Unable to retrieve coordinates for '{location}' to check weather alerts.";
                }

                var alertsData = await _weatherRepository.GetWeatherAlertsAsync(weatherData.Coord.Lat, weatherData.Coord.Lon);
                
                if (alertsData == null)
                {
                    return $"Unable to retrieve weather alerts for '{location}'.";
                }

                return FormatWeatherAlerts(alertsData, location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather alerts for location: {Location}", location);
                return $"An error occurred while retrieving weather alerts for '{location}'.";
            }
        }

        private string FormatCurrentWeather(WeatherResponse weather)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"🌤️ Current Weather in {weather.Name}, {weather.Sys?.Country}");
            sb.AppendLine("=" + new string('=', 40));
            
            if (weather.Weather?.Any() == true)
            {
                var mainWeather = weather.Weather.First();
                sb.AppendLine($"Condition: {mainWeather.Main} - {mainWeather.Description}");
            }
            
            if (weather.Main != null)
            {
                sb.AppendLine($"Temperature: {weather.Main.Temp:F1}°C (feels like {weather.Main.FeelsLike:F1}°C)");
                sb.AppendLine($"Min/Max: {weather.Main.TempMin:F1}°C / {weather.Main.TempMax:F1}°C");
                sb.AppendLine($"Humidity: {weather.Main.Humidity}%");
                sb.AppendLine($"Pressure: {weather.Main.Pressure} hPa");
            }
            
            if (weather.Wind != null)
            {
                sb.AppendLine($"Wind: {weather.Wind.Speed} m/s at {weather.Wind.Deg}°");
                if (weather.Wind.Gust.HasValue)
                {
                    sb.AppendLine($"Wind Gust: {weather.Wind.Gust:F1} m/s");
                }
            }
            
            if (weather.Clouds != null)
            {
                sb.AppendLine($"Cloudiness: {weather.Clouds.All}%");
            }
            
            sb.AppendLine($"Visibility: {weather.Visibility / 1000.0:F1} km");
            
            if (weather.Sys != null && weather.Sys.Sunrise.HasValue && weather.Sys.Sunset.HasValue)
            {
                var sunrise = DateTimeOffset.FromUnixTimeSeconds(weather.Sys.Sunrise.Value).ToString("HH:mm");
                var sunset = DateTimeOffset.FromUnixTimeSeconds(weather.Sys.Sunset.Value).ToString("HH:mm");
                sb.AppendLine($"Sunrise: {sunrise} | Sunset: {sunset}");
            }
            
            var lastUpdated = DateTimeOffset.FromUnixTimeSeconds(weather.Dt).ToString("yyyy-MM-dd HH:mm UTC");
            sb.AppendLine($"Last Updated: {lastUpdated}");
            
            return sb.ToString();
        }

        private string FormatWeatherForecast(ForecastResponse forecast, int days)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"🌦️ {days}-Day Weather Forecast for {forecast.City?.Name}, {forecast.City?.Country}");
            sb.AppendLine("=" + new string('=', 50));
            
            if (forecast.List?.Any() != true)
            {
                return "No forecast data available.";
            }

            // Group forecast data by date
            var groupedByDate = forecast.List
                .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Dt).Date)
                .Take(days)
                .ToList();

            foreach (var dayGroup in groupedByDate)
            {
                var date = dayGroup.Key;
                var dayItems = dayGroup.ToList();
                
                sb.AppendLine($"\n📅 {date:dddd, MMMM dd}");
                sb.AppendLine(new string('-', 30));
                
                // Get min/max temperatures for the day
                var minTemp = dayItems.Min(item => item.Main?.TempMin ?? 0);
                var maxTemp = dayItems.Max(item => item.Main?.TempMax ?? 0);
                
                // Get most common weather condition
                var commonWeather = dayItems
                    .SelectMany(item => item.Weather ?? new List<Weather>())
                    .GroupBy(w => w.Main)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key ?? "Unknown";
                
                sb.AppendLine($"Temperature: {minTemp:F1}°C - {maxTemp:F1}°C");
                sb.AppendLine($"Condition: {commonWeather}");
                
                // Show detailed forecast for key time periods
                var morningItem = dayItems.FirstOrDefault(item => 
                    DateTimeOffset.FromUnixTimeSeconds(item.Dt).Hour >= 6 && 
                    DateTimeOffset.FromUnixTimeSeconds(item.Dt).Hour <= 12);
                    
                var afternoonItem = dayItems.FirstOrDefault(item => 
                    DateTimeOffset.FromUnixTimeSeconds(item.Dt).Hour >= 12 && 
                    DateTimeOffset.FromUnixTimeSeconds(item.Dt).Hour <= 18);
                    
                var eveningItem = dayItems.FirstOrDefault(item => 
                    DateTimeOffset.FromUnixTimeSeconds(item.Dt).Hour >= 18);

                if (morningItem != null)
                {
                    sb.AppendLine($"  Morning: {morningItem.Main?.Temp:F1}°C, {morningItem.Weather?.FirstOrDefault()?.Description ?? "N/A"}");
                }
                if (afternoonItem != null)
                {
                    sb.AppendLine($"  Afternoon: {afternoonItem.Main?.Temp:F1}°C, {afternoonItem.Weather?.FirstOrDefault()?.Description ?? "N/A"}");
                }
                if (eveningItem != null)
                {
                    sb.AppendLine($"  Evening: {eveningItem.Main?.Temp:F1}°C, {eveningItem.Weather?.FirstOrDefault()?.Description ?? "N/A"}");
                }
                
                // Average humidity and wind for the day
                var avgHumidity = dayItems.Average(item => item.Main?.Humidity ?? 0);
                var avgWindSpeed = dayItems.Average(item => item.Wind?.Speed ?? 0);
                
                sb.AppendLine($"  Humidity: {avgHumidity:F0}% | Wind: {avgWindSpeed:F1} m/s");
                
                // Precipitation probability
                var maxPop = dayItems.Max(item => item.Pop) * 100;
                if (maxPop > 0)
                {
                    sb.AppendLine($"  Precipitation: {maxPop:F0}% chance");
                }
            }
            
            return sb.ToString();
        }

        private string FormatWeatherAlerts(WeatherAlertsResponse alerts, string location)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"⚠️ Weather Alerts for {location}");
            sb.AppendLine("=" + new string('=', 35));
            
            if (alerts.Alerts?.Any() != true)
            {
                sb.AppendLine("✅ No active weather alerts for this location.");
                return sb.ToString();
            }
            
            for (int i = 0; i < alerts.Alerts.Count; i++)
            {
                var alert = alerts.Alerts[i];
                
                sb.AppendLine($"\n🚨 Alert #{i + 1}: {alert.Event}");
                sb.AppendLine(new string('-', 25));
                
                if (!string.IsNullOrEmpty(alert.SenderName))
                {
                    sb.AppendLine($"Issued by: {alert.SenderName}");
                }
                
                var startTime = DateTimeOffset.FromUnixTimeSeconds(alert.Start).ToString("yyyy-MM-dd HH:mm UTC");
                var endTime = DateTimeOffset.FromUnixTimeSeconds(alert.End).ToString("yyyy-MM-dd HH:mm UTC");
                
                sb.AppendLine($"Active from: {startTime}");
                sb.AppendLine($"Until: {endTime}");
                
                if (!string.IsNullOrEmpty(alert.Description))
                {
                    sb.AppendLine($"Description: {alert.Description}");
                }
                
                if (alert.Tags?.Any() == true)
                {
                    sb.AppendLine($"Tags: {string.Join(", ", alert.Tags)}");
                }
            }
            
            return sb.ToString();
        }
    }
}
