
using WDModel.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WAService.Services
{
    public class WeatherApiService
    {
        private readonly HttpClient _httpClient;

        public WeatherApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherData?> GetHumidityDataAsync(double latitude, double longitude, string startDate, string endDate, string timezone = "auto")
        {
            string url = $"https://archive-api.open-meteo.com/v1/archive?latitude={latitude}&longitude={longitude}&start_date={startDate}&end_date={endDate}&daily=relative_humidity_2m_mean&timezone={timezone}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API call failed: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var res = JsonSerializer.Deserialize<WeatherData>(content, options);
            Console.WriteLine($"error:{res.daily.time[0]}");
            return res;
         }
    }
}
