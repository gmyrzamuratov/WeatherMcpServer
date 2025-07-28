# 🌤️ Weather MCP Server - Quick Start Guide

Get real-time weather data, forecasts, and alerts through the Model Context Protocol in just a few minutes!

## 📋 Prerequisites

- **.NET 8.0 or later** - [Download here](https://dotnet.microsoft.com/download)
- **OpenWeatherMap API Key** - Free tier available
- **MCP-compatible client** (Claude Desktop, VS Code with Copilot, etc.)

## 🚀 Quick Setup (5 minutes)

### Step 1: Get Your Free API Key

1. Visit [OpenWeatherMap API](https://openweathermap.org/api)
2. Create a free account (no credit card required)
3. Navigate to **API Keys** in your dashboard
4. Copy your API key (starts with a string of letters/numbers)

> 💡 **Free Tier Includes**: 1,000 API calls/day, current weather, 5-day forecasts

### Step 2: Set Your API Key

You can set your API key as an environment variable using the export command:

```bash
export OPENWEATHER_API_KEY="your_actual_api_key_here"
```

Alternatively, you can add it directly to your MCP configuration file in the next steps.

> 🔐 **Security Note**: Never commit API keys to version control. Keep them secure in your MCP config files or environment variables.

### Step 3: Test the Server

```bash
# Navigate to project directory
cd /path/to/WeatherMcpServer

# Restore dependencies (first time only)
dotnet restore

# Build the project
dotnet build

# Run the server
dotnet run
```

✅ **Success indicators:**
- No error messages about missing API key
- Server starts without compilation errors
- Ready to accept MCP connections

### VS Code (with Copilot Chat)

Create/edit `.vscode/mcp.json` in your workspace:

```json
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/absolute/path/to/WeatherMcpServer"
      ],
    }
  }
}
```

## 🎯 Test Your Setup

Try these example queries in your MCP client:

### Current Weather
- *"What's the current weather in London?"*
- *"How's the weather in New York right now?"*
- *"Give me today's weather for Tokyo, Japan"*

### Weather Forecasts
- *"Show me a 3-day forecast for Paris"*
- *"What will the weather be like in Sydney over the next 5 days?"*
- *"Give me this week's weather forecast for Miami"*

### Weather Alerts
- *"Are there any weather alerts for San Francisco?"*
- *"Check for storm warnings in Florida"*
- *"Show weather alerts for my location"* (if location is specified)

## 🏗️ Project Architecture

Understanding the codebase structure:

```
WeatherMcpServer/
├── 📁 Interfaces/       # Service contracts
│   ├── IWeatherRepository.cs
│   └── IWeatherService.cs
├── 📁 Models/           # Data structures
│   └── WeatherModels.cs
├── 📁 Repositories/     # API integration
│   └── OpenWeatherMapRepository.cs
├── 📁 Services/         # Business logic
│   └── WeatherService.cs
├── 📁 Tools/           # MCP tool definitions
│   └── WeatherTools.cs
└── 📄 Program.cs       # Application entry point
```

## 🐛 Troubleshooting

### Common Issues

#### ❌ "API key is not configured"

**Causes & Solutions:**
- Environment variable not set → Set `OPENWEATHER_API_KEY`
- Typo in variable name → Ensure exact spelling (case-sensitive)
- Variable not loaded → Restart terminal/IDE after setting
- Wrong API key → Verify key in OpenWeatherMap dashboard

**Debug steps:**
```bash
# Check if variable is set
echo $OPENWEATHER_API_KEY  # macOS/Linux
echo %OPENWEATHER_API_KEY%  # Windows CMD
```

#### ❌ "Unable to retrieve weather data"

**Causes & Solutions:**
- Invalid location → Use standard city names ("London", "New York")
- Ambiguous location → Add country code ("Paris,FR", "London,UK")
- Network issues → Check internet connection
- API limits exceeded → Wait for daily reset or upgrade plan

**Supported location formats:**
- City name: `"London"`
- City, Country: `"London,UK"`
- City, State, Country: `"New York,NY,US"`

#### ❌ ".NET compilation errors"

**Solutions:**
- Install .NET 8.0+ SDK
- Run `dotnet restore` to get dependencies
- Check for syntax errors in recent changes
- Clean and rebuild: `dotnet clean && dotnet build`

#### ❌ "Rate limit exceeded"

**Free tier limits:**
- 1,000 calls/day
- 60 calls/minute

**Solutions:**
- Wait for daily reset (midnight UTC)
- Upgrade OpenWeatherMap plan
- Implement caching (for development)
- Reduce query frequency

## 📈 Usage Tips

### Best Practices
- Use specific city names to avoid ambiguity
- Include country codes for common city names
- Cache results during development to save API calls
- Monitor your API usage in OpenWeatherMap dashboard

### Location Examples
```
✅ Good: "London,UK", "Paris,FR", "New York,NY,US"
✅ Good: "Tokyo", "Sydney", "Vancouver"
❌ Avoid: "Springfield" (too many exist)
❌ Avoid: "Paris" (could be France or Texas)
```

### Supported Weather Data
- **Current Conditions**: Temperature, humidity, pressure, wind
- **5-Day Forecasts**: Daily breakdowns with 3-hour intervals
- **Weather Alerts**: Warnings, watches, advisories (where available)
- **Global Coverage**: 200,000+ cities worldwide

## 🔗 Next Steps

- **Customize**: Modify weather data formatting in `WeatherService.cs`
- **Extend**: Add new weather endpoints in `OpenWeatherMapRepository.cs`
- **Deploy**: Consider containerizing for production use
- **Monitor**: Set up logging and monitoring for production environments

## 📚 Additional Resources

- [OpenWeatherMap API Documentation](https://openweathermap.org/api)
- [Model Context Protocol Specification](https://spec.modelcontextprotocol.io/)
- [.NET MCP SDK Documentation](https://www.nuget.org/packages/ModelContextProtocol)

---

**Need help?** Check the main [README.md](./README.md) for detailed documentation and development guidelines.
