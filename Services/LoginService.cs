using SigninService.Models;
using SignModel.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UDService.Services;
namespace SigninService.Services
{
    public class Loginservice
    {
        private readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJ1dmloZnJjYmFicGd0eHllcGZ5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczMTMyODAsImV4cCI6MjA2Mjg4OTI4MH0.Xnqqwg08fGbznn-Wk4kfRwU07CdVV6IecH86rpsaLl4";
        private readonly string BaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
        private readonly HttpClient httpc;
        private readonly Userdata _userData;
        public Loginservice(Userdata userData)
        {
            httpc = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _userData = userData;      
        }
        private string accessToken = string.Empty;
        private string uid = string.Empty;

        public async Task<bool> CheckDataAsync(string Email, string Password)
        {
            try
            {
                string endpoint = "/auth/v1/token?grant_type=password";
                string Pmail = Email;
                Console.WriteLine(Pmail);
                var signInData = new
                {
                    email = Email,
                    password = Password
                };
                
                string jsonContent = JsonSerializer.Serialize(signInData);
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("apikey", ApiKey);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpc.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Login is successful");
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var authResponse = JsonSerializer.Deserialize<AuthResponseModel>(responseBody, options);
                    if (authResponse?.access_token != null)
                    {
                        accessToken = authResponse.access_token;
                        uid = authResponse.user?.id;
                        string table = "profile";
                        string filter = $"uid=eq.{uid}";
                        var request1 = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table}?{filter}");
                        request1.Headers.Add("apikey", ApiKey);
                        request1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        var response1 = await httpc.SendAsync(request1);
                        var result1 = await response1.Content.ReadAsStringAsync();

                        if (response1.IsSuccessStatusCode)
                        {
                            var options1 = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var nameData = JsonSerializer.Deserialize<List<FavouriteResult1>>(result1, options1);
                            Console.WriteLine("name fetched");
                            Console.WriteLine(nameData?.FirstOrDefault()?.name ?? "No name found");
                            
                            string table1 = "user_details";
                            string filter1 = $"uid=eq.{uid}";
                            var request2 = new HttpRequestMessage(HttpMethod.Get, $"/rest/v1/{table1}?{filter1}");
                            request2.Headers.Add("apikey", ApiKey);
                            request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                            var response2 = await httpc.SendAsync(request2);
                            var result2 = await response2.Content.ReadAsStringAsync();

                            if (response2.IsSuccessStatusCode)
                            {
                                var options2 = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                                var themeData2 = JsonSerializer.Deserialize<List<UserDetailsResult>>(result2, options2);

                                bool userTheme = themeData2?.FirstOrDefault()?.theme ?? false; // Default to false if null
                                Console.WriteLine($"Theme fetched: {userTheme}");
                                await _userData.updateDetails(nameData?.FirstOrDefault()?.name, uid, accessToken,userTheme);
                                return true;
                            }
                            else
                            {
                                Console.WriteLine($"Failed to fetch theme. Status: {response2.StatusCode}");
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
            public class FavouriteResult1
            {
                public string name { get; set; }
            }
        public class UserDetailsResult
        {
            public bool theme { get; set; }
        }

    }
}

