using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UDService.Services;

namespace WSService.Services
{
    public class WeatherService
    {
        private readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJ1dmloZnJjYmFicGd0eHllcGZ5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczMTMyODAsImV4cCI6MjA2Mjg4OTI4MH0.Xnqqwg08fGbznn-Wk4kfRwU07CdVV6IecH86rpsaLl4";
        private readonly string BaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
        private readonly HttpClient httpc;
        private readonly Userdata _userData;
        public WeatherService(Userdata userData)
        {
            httpc = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _userData = userData;
        }
        public class FavouriteResult
        {
            public string[] favourite { get; set; }
        }
        public async Task<bool> GetBoolAsync(string city)
        {
            var fav_city = await GetFavouritesAsync();
            if (!fav_city.Contains(city))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        public async Task AddFavAsync(string city)
        {
            var fav_city = await GetFavouritesAsync();
            if(!fav_city.Contains(city))
            {
                fav_city.Add(city);
                await UpdateProfileAsync(fav_city);
            }
            else if(fav_city.Contains(city))
            {
                fav_city.Remove(city);
                await UpdateProfileAsync(fav_city);
            }
        }
        public async Task<List<string>> GetFavouritesAsync()
        {
            string table = "profile";
            string filter = $"uid=eq.{_userData.Uid}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);

            var response = await httpc.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var favourites = JsonSerializer.Deserialize<List<FavouriteResult>>(result, options);
                // Console.WriteLine(favourites);
                return favourites?.FirstOrDefault()?.favourite?.ToList() ?? new List<string>();
            }
            else
            {
                Console.WriteLine("Failed to fetch favourites.");
                return new List<string>();
            }

        }
        public async Task<bool> UpdateProfileAsync(List<string> city)
        {
            string table = "profile"; // or your table name
            string filter = $"uid=eq.{_userData.Uid}";
            string[] fav = city.ToArray();
            var updateData = new
            {
                favourite = fav,
            };

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);
            request.Headers.Add("Prefer", "return=representation");

            string json = JsonSerializer.Serialize(updateData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpc.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Update successful:");
                Console.WriteLine(body);
                return true;
            }
            else
            {
                Console.WriteLine("Failed to update:");
                Console.WriteLine(body);
                return false;
            }
        }
        public async Task<bool> UpdateAlertAsync(string pemail,float temperature,float hum,List<string> city)
        {
            string table = "user_details"; // or your table name
            string filter = $"uid=eq.{_userData.Uid}";
            string[] fav = city.ToArray();
            var updateData = new
            {
                email = pemail,
                temp = temperature,
                humidity = hum,
                cities = fav,
            };

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);
            request.Headers.Add("Prefer", "return=representation");

            string json = JsonSerializer.Serialize(updateData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpc.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Update of alert successful:");
                Console.WriteLine(body);
                return true;
            }
            else
            {
                Console.WriteLine("Failed to update alert:");
                Console.WriteLine(body);
                return false;
            }
        }
    }
}
