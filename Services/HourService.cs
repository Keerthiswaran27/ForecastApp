
using HFModel.Models;
using RHModel.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HSService.Services
{
    public class HourService
    {
        private readonly HttpClient _http;

        public HourService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<HourlyForecast>> GetHourlyForecastAsync(double latitude, double longitude)
        {
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,weathercode&timezone=auto";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var root = await _http.GetFromJsonAsync<RootHourly>(url, options);

                var result = new List<HourlyForecast>();

                if (root?.hourly != null)
                {
                    for (int i = 0; i < root.hourly.time.Count; i++)
                    {
                        result.Add(new HourlyForecast
                        {
                            Time = DateTime.Parse(root.hourly.time[i]),
                            Temperature = root.hourly.temperature_2m[i],
                            WeatherCode = root.hourly.weathercode[i]
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching hourly forecast: " + ex.Message);
                return new List<HourlyForecast>();
            }
        }
    }
}
