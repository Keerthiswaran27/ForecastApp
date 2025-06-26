using HDModel.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using UDService.Services;
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
        public HistoryService(Userdata userData)
        {
            httpc = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _userData = userData;
        }
        
        public async Task<List<Historydetails>> GetHistoryAsync()
        {
            string table = "history";
            string filter = $"uid=eq.{_userData.Uid}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);

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
                uid = _userData.Uid,
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
            string filter = $"uid=eq.{_userData.Uid}"; // use the same filter condition

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);

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
