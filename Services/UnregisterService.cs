using WRModel.Models;
using DFModel.Models;
using System.Text.Json;
using System.Net.Http.Json;



namespace URService.Services
{
    public class UnregisterService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "d44e84653bedfb2f9682a4874783732d";

        public UnregisterService(HttpClient http)
        {
            _http = http;
        }
        public List<DailyForecast> forecastList = new();

        public async Task<List<DailyForecast>> GetWeather(string city)
        {
            try
            {
                forecastList.Clear();
                string apiKey = "d44e84653bedfb2f9682a4874783732d";
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";

                var http = new HttpClient();
                var result = await _http.GetFromJsonAsync<WeatherResponse>(url);

                var grouped = result!.List!
                    .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Dt).Date)
                    .Take(3);

                foreach (var group in grouped)
                {
                    // Pick 12:00 PM entry if exists, otherwise take average
                    var noonEntry = group.FirstOrDefault(x =>
                        DateTimeOffset.FromUnixTimeSeconds(x.Dt).Hour == 12);

                    var entry = noonEntry ?? group.First();

                    forecastList.Add(new DailyForecast
                    {
                        Date = DateTimeOffset.FromUnixTimeSeconds(entry.Dt).ToString("yyyy-MM-dd"),
                        Temp = Math.Round(entry.Main!.Temp),
                        Humidity = entry.Main.Humidity,
                        Condition = entry.Weather?.FirstOrDefault()?.Main ?? "Unknown",
                        WindSpeed = Math.Round(entry.Wind?.Speed ?? 0)
                    });
                    
                }
                return forecastList ?? new List<DailyForecast>();;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return forecastList;
            }
        }
    }
}
