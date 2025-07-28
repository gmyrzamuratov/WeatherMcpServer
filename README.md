# Weather MCP Server

A real weather Model Context Protocol (MCP) server that provides current weather data, forecasts, and weather alerts using the OpenWeatherMap API.

## Features

- **Current Weather**: Get real-time weather conditions for any city worldwide
- **Weather Forecast**: Get weather forecasts for up to 5 days
- **Weather Alerts**: Get weather warnings and alerts for specific locations
- **Global Coverage**: Support for cities worldwide
- **Comprehensive Data**: Temperature, humidity, wind, precipitation, and more

## Tools Available

### GetCurrentWeather
Get current weather conditions for a specified location.
- **Parameters**: `location` (string) - Name of the city or location
- **Returns**: Formatted current weather information including temperature, conditions, humidity, wind, and more

### GetWeatherForecast
Get weather forecast for a specified location for the next few days.
- **Parameters**: 
  - `location` (string) - Name of the city or location
  - `days` (int, optional) - Number of days to forecast (1-5, default is 3)
- **Returns**: Detailed weather forecast with daily breakdown

### GetWeatherAlerts
Get weather alerts and warnings for a specified location.
- **Parameters**: `location` (string) - Name of the city or location
- **Returns**: List of active weather alerts, warnings, and advisories

## Prerequisites

- .NET 8.0 or later
- OpenWeatherMap API key (free tier available)

## Setup

### 1. Get OpenWeatherMap API Key

1. Visit [OpenWeatherMap](https://openweathermap.org/api)
2. Sign up for a free account
3. Generate an API key from your account dashboard
4. Note: The free tier includes:
   - Current weather data
   - 5-day weather forecast
   - 1,000 API calls per day

### 2. Configure Environment Variables

Set the following environment variable with your OpenWeatherMap API key:

```bash
export OPENWEATHER_API_KEY="your_api_key_here"
```

Or on Windows:
```cmd
set OPENWEATHER_API_KEY=your_api_key_here
```

### 3. Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the server
dotnet run
```

## Usage Examples

Once the MCP server is running, you can use it with any MCP-compatible client (like Claude Desktop). Here are some example queries:

- "What's the current weather in London?"
- "Give me a 5-day forecast for Tokyo"
- "Are there any weather alerts for Miami?"
- "What's the temperature in Paris right now?"

## Configuration

### Environment Variables

- `OPENWEATHER_API_KEY` (required) - Your OpenWeatherMap API key
- `WEATHER_CHOICES` (optional) - Legacy environment variable for the random weather tool

### API Limits

The free OpenWeatherMap plan includes:
- 1,000 API calls per day
- Current weather data
- 5-day / 3-hour forecast
- Weather alerts (requires One Call API 3.0 subscription for full functionality)

## Architecture

The server follows a clean architecture pattern:

- **Models**: Data transfer objects for API responses
- **Interfaces**: Service and repository contracts
  - `IWeatherRepository`: Repository interface for weather data access
  - `IWeatherService`: Service interface for business logic
- **Repositories**: Data access layer implementations
  - `OpenWeatherMapRepository`: OpenWeatherMap API integration
- **Services**: Business logic layer implementations
  - `WeatherService`: Weather data processing and formatting
- **Tools**: `WeatherTools` - MCP tool implementations## Error Handling

The server includes comprehensive error handling:
- Invalid location names
- API rate limiting
- Network connectivity issues
- Missing API key configuration
- Malformed API responses

## Weather Data Sources

- **Current Weather**: OpenWeatherMap Current Weather API
- **Forecasts**: OpenWeatherMap 5-day Forecast API
- **Alerts**: OpenWeatherMap One Call API 3.0 (requires subscription for full alerts)

## Developing locally

To test this MCP server from source code (locally) without using a built MCP server package, you can configure your IDE to run the project directly using `dotnet run`.

```json
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "<PATH TO PROJECT DIRECTORY>"
      ]
    }
  }
}
```

## Testing the MCP Server

Once configured with your OpenWeatherMap API key, you can ask questions like:
- "What's the current weather in London?"
- "Give me a 3-day forecast for New York"
- "Are there any weather alerts for Miami?"

## Development

### Adding New Features

1. Add new models to `Models/WeatherModels.cs`
2. Define new contracts in the `Interfaces` folder
3. Extend `OpenWeatherMapRepository` in the `Repositories` folder
4. Update `WeatherService` in the `Services` folder
5. Add new tools to `WeatherTools.cs`

### Testing

You can test the server by running it and connecting it to an MCP client like Claude Desktop or VS Code with Copilot Chat.

## More information

.NET MCP servers use the [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol) C# SDK. For more information about MCP:

- [Official Documentation](https://modelcontextprotocol.io/)
- [Protocol Specification](https://spec.modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)

Refer to the VS Code or Visual Studio documentation for more information on configuring and using MCP servers:

- [Use MCP servers in VS Code (Preview)](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)
- [Use MCP servers in Visual Studio (Preview)](https://learn.microsoft.com/visualstudio/ide/mcp-servers)

## License

This project is provided as-is for educational and development purposes.

## Support

For issues related to:
- OpenWeatherMap API: Check [OpenWeatherMap documentation](https://openweathermap.org/api)
- MCP Protocol: Check [Model Context Protocol documentation](https://modelcontextprotocol.io/)
- This server: Create an issue in the repository
