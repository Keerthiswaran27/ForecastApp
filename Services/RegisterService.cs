using MFModel.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using WRModel.Models;

namespace RSService.Services
{
    public class RegisterService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "d44e84653bedfb2f9682a4874783732d";
        private readonly RegisterService _registerService;
        private readonly IJSRuntime js;
        private string accessToken1 = "";
        private string uid1 = "";
        public RegisterService(HttpClient http, IJSRuntime jsRuntime)
        {
            _http = http;
            js = jsRuntime;
        }

        public async Task<List<MainForecase>> GetForecastAsync(string city, int dayCount)
        {
            var forecastList = new List<MainForecase>();
            accessToken1 = await js.InvokeAsync<string>("sessionStorage.getItem", "accessToken");
            uid1 = await js.InvokeAsync<string>("sessionStorage.getItem", "uid");
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";
                var result = await _http.GetFromJsonAsync<WeatherResponse>(url);

                if (result == null || result.List == null || result.City == null)
                    return forecastList;

                var grouped = result.List
                    .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Dt).Date)
                    .Take(dayCount);

                foreach (var group in grouped)
                {
                    var entry = group.FirstOrDefault(x =>
                        DateTimeOffset.FromUnixTimeSeconds(x.Dt).Hour == 12)
                        ?? group.First();

                    forecastList.Add(new MainForecase
                    {
                        CityName = result.City.Name,
                        Date = DateOnly.Parse(DateTimeOffset.FromUnixTimeSeconds(entry.Dt).ToString("yyyy-MM-dd")),
                        Temp = Math.Round(entry.Main.Temp),
                        FeelsLike = Math.Round(entry.Main.Feels_Like),
                        Pressure = entry.Main.Pressure,
                        Humidity = entry.Main.Humidity,
                        WindSpeed = Math.Round(entry.Wind.Speed),
                        WindDeg = entry.Wind.Deg,
                        Lat = result.City.Coord.Lat,
                        Lon = result.City.Coord.Lon,
                        Country = result.City.Country,
                        Main = entry.Weather.FirstOrDefault()?.Main ?? "Unknown",
                        Description = entry.Weather.FirstOrDefault()?.Description ?? "Unknown"
                    });
                }

                return forecastList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return forecastList;
            }
        }
    }
}
