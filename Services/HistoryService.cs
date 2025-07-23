using DWModel.Models;
using HDModel.Models;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using UDService.Services;
using WDModel.Models;
using WRModel.Models;
using static System.Net.WebRequestMethods;
using static WSService.Services.WeatherService;

namespace HService.Services
{
    public class HistoryService
    {
        private readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJ1dmloZnJjYmFicGd0eHllcGZ5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczMTMyODAsImV4cCI6MjA2Mjg4OTI4MH0.Xnqqwg08fGbznn-Wk4kfRwU07CdVV6IecH86rpsaLl4";
        private readonly string BaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
        private readonly HttpClient httpc;
        private readonly Userdata _userData;
        private readonly IJSRuntime js;
        private string accessToken1 = "";
        private string uid1 = "";
        public HistoryService(Userdata userData, IJSRuntime jsRuntime)
        {
            httpc = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _userData = userData;
            js = jsRuntime;
        }
        public async Task<List<DailyWeather>> GetMonthlyWeatherAsync(string stationId, string start, string end)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://meteostat.p.rapidapi.com/stations/daily?station={stationId}&start={start}&end={end}")
            };
            request.Headers.Add("X-RapidAPI-Key", "b9cdcb2588msh868b74a12923e2cp179e77jsn8024bda77991");
            request.Headers.Add("X-RapidAPI-Host", "meteostat.p.rapidapi.com");

            using var response = await httpc.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var list = new List<DailyWeather>();

            var json = JsonDocument.Parse(body);
            var data = json.RootElement.GetProperty("data");
            foreach (var item in data.EnumerateArray())
            {
                list.Add(new DailyWeather
                {
                    Date = DateTime.Parse(item.GetProperty("date").GetString() ?? ""),
                    Tavg = item.GetProperty("tavg").GetDouble(),
                    Tmin = item.GetProperty("tmin").GetDouble(),
                    Tmax = item.GetProperty("tmax").GetDouble(),
                    Prcp = item.GetProperty("prcp").GetDouble()
                });
            }

            return list;
        }
        
        public async Task<Location> GetLocationAsync(string cityName)
        {
            var url1 = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json";

            var request1 = new HttpRequestMessage(HttpMethod.Get, url1);
            request1.Headers.Add("User-Agent", "ForecastApp/1.0 (keerthiswarankrk@gmail.com)");

            using var response1 = await httpc.SendAsync(request1);
            response1.EnsureSuccessStatusCode();

            var body1 = await response1.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(body1);
            var first = doc.RootElement[0];
            Location result = new Location();
            result.Lat = first.GetProperty("lat").GetString();
            result.Lon = first.GetProperty("lon").GetString();
            return result;
        }

        public async Task<string> GetStationIdAsync(string cityName)
        {
            var url1 = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json";

            var request1 = new HttpRequestMessage(HttpMethod.Get, url1);
            request1.Headers.Add("User-Agent", "ForecastApp/1.0 (keerthiswarankrk@gmail.com)");

            using var response1 = await httpc.SendAsync(request1);
            response1.EnsureSuccessStatusCode();

            var body1 = await response1.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(body1);
            var first = doc.RootElement[0];


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://meteostat.p.rapidapi.com/stations/nearby?lat={first.GetProperty("lat").GetString()}&lon={first.GetProperty("lon").GetString()}"),
            };
            request.Headers.Add("X-RapidAPI-Key", "b9cdcb2588msh868b74a12923e2cp179e77jsn8024bda77991");
            request.Headers.Add("X-RapidAPI-Host", "meteostat.p.rapidapi.com");

            using var response = await httpc.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // If 404 or other error, gracefully handle it
                return "not found";
            }

            var body = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(body);

            if (!json.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
            {
                return "not found";
            }

            var stationId = data[0].GetProperty("id").GetString();

            return stationId ?? "not found";
        }


        public async Task<List<Historydetails>> GetHistoryAsync()
        {
            accessToken1 = await js.InvokeAsync<string>("sessionStorage.getItem", "accessToken");
            uid1 = await js.InvokeAsync<string>("sessionStorage.getItem", "uid");
            string table = "history";
            string filter = $"uid=eq.{uid1}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);

            var response = await httpc.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var history = JsonSerializer.Deserialize<List<Historydetails>>(result, options);
                return history ?? new List<Historydetails>() ;
            }
            else
            {
                Console.WriteLine("Failed to fetch favourites.");
                return new List<Historydetails>();
            }
        }
        public async Task<bool> PostHistoryAsync(string city1,double temperature,int humidity,double Windspeed,string Condition)
        {
            
            var userInsert = new
            {
                uid = uid1,
                city = city1,
                temp = temperature,
                hum = humidity,
                windspeed =Windspeed,
                condition = Condition
            };
            string insertJson = JsonSerializer.Serialize(userInsert);

            var insertRequest = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/history");
            insertRequest.Headers.Add("apikey", ApiKey);
            insertRequest.Headers.Add("Authorization", $"Bearer {ApiKey}");
            //insertRequest.Headers.Add("Prefer", "return=representation");
            insertRequest.Content = new StringContent(insertJson, Encoding.UTF8, "application/json");

            var insertResponse = await httpc.SendAsync(insertRequest);
            var insertResponseBody = await insertResponse.Content.ReadAsStringAsync();

            if (insertResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Update of history successful:");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to update history");
                return false;
            }
        }

        public async Task DeleteHistoryAsync()
        {
            string table = "history";
            string filter = $"uid=eq.{uid1}"; // use the same filter condition

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);

            var response = await httpc.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Deleted successfully from history.");
            }
            else
            {
                Console.WriteLine("Failed to delete. Response:");
                Console.WriteLine(result);
            }
        }
    }
}
