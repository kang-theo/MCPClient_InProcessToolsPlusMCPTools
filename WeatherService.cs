using System.ComponentModel;
using Microsoft.Extensions.AI;

public class WeatherService
{
  [Description("Get current weather for a specific location")]
  public async Task<string> GetWeatherAsync(
    [Description("The city name")] string city,
    [Description("The country code (optional)")] string? country = null)
  {
    // Simulate weather API call
    await Task.Delay(100);
    var temperature = Random.Shared.Next(-10, 35);
    var conditions = new[] { "Sunny", "Cloudy", "Rainy", "Snowy" };
    var condition = conditions[Random.Shared.Next(conditions.Length)];
    
    var location = string.IsNullOrEmpty(country) ? city : $"{city}, {country}";
    return $"Weather in {location}: {temperature}°C, {condition}";
  }
}