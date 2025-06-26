using HDModel.Models;
using PDModel.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using UDService.Services;

namespace PSService.Services
{
    public class ProfileService
    {
        private readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJ1dmloZnJjYmFicGd0eHllcGZ5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczMTMyODAsImV4cCI6MjA2Mjg4OTI4MH0.Xnqqwg08fGbznn-Wk4kfRwU07CdVV6IecH86rpsaLl4";
        private readonly string BaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
        private readonly HttpClient httpc;
        private readonly Userdata _userData;
        public ProfileService(Userdata userData)
        {
            httpc = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _userData = userData;
        }
        public async Task<ProfileDetails> GetAlertDetailsAsync()
        {
            string table = "user_details";
            string filter = $"uid=eq.{_userData.Uid}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);

            var response = await httpc.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var profiles = JsonSerializer.Deserialize<List<ProfileDetails>>(result, options);
                return profiles?.FirstOrDefault() ?? new ProfileDetails();
            }
            else
            {
                Console.WriteLine("Failed to fetch favourites.");
                return new ProfileDetails();
            }
        }
        public async Task<AuthDetails> GetProfileDetailsAsync()
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
                var profiles = JsonSerializer.Deserialize<List<AuthDetails>>(result, options);
                return profiles?.FirstOrDefault() ?? new AuthDetails();
            }
            else
            {
                Console.WriteLine("Failed to fetch favourites.");
                return new AuthDetails();
            }
        }
    }
}
