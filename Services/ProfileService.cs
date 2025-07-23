using HDModel.Models;
using Microsoft.JSInterop;
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
        private readonly IJSRuntime js;
        private string accessToken1 = "";
        private string uid1 = "";

        public ProfileService(Userdata userData, IJSRuntime jsRuntime)
        {
            httpc = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _userData = userData;
            js = jsRuntime;
        }
        public async Task<ProfileDetails> GetAlertDetailsAsync()
        {
            accessToken1 = await js.InvokeAsync<string>("sessionStorage.getItem", "accessToken");
            uid1 = await js.InvokeAsync<string>("sessionStorage.getItem", "uid");
            string table = "user_details";
            string filter = $"uid=eq.{uid1}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);

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
            string filter = $"uid=eq.{uid1}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);

            var response = await httpc.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var profiles = JsonSerializer.Deserialize<List<AuthDetails>>(result, options);
                Console.WriteLine($"user :{profiles?.FirstOrDefault()}");

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
